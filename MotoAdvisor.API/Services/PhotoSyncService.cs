using Microsoft.EntityFrameworkCore;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.API.Services;

/// <summary>
/// Runs once at startup: copies images from the Photos source directory into
/// wwwroot/photos/{ModelFolder}/ and refreshes MotorcycleImage records in the DB.
/// </summary>
public class PhotoSyncService : IHostedService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWebHostEnvironment  _env;
    private readonly IConfiguration       _config;
    private readonly ILogger<PhotoSyncService> _logger;

    public PhotoSyncService(
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment  env,
        IConfiguration       config,
        ILogger<PhotoSyncService> logger)
    {
        _scopeFactory = scopeFactory;
        _env          = env;
        _config       = config;
        _logger       = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var sourceRoot = _config["Photos:SourcePath"] ?? string.Empty;
            sourceRoot = sourceRoot.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            if (!Directory.Exists(sourceRoot))
            {
                _logger.LogWarning("PhotoSync: source directory not found: {Path}", sourceRoot);
                return;
            }

            var wwwPhotos = Path.Combine(_env.WebRootPath, "photos");
            Directory.CreateDirectory(wwwPhotos);

            // When both mounts point to the same host directory the paths resolve
            // identically — copying would cause the IOException we're avoiding.
            var sameDir = Path.GetFullPath(sourceRoot) == Path.GetFullPath(wwwPhotos);
            if (sameDir)
                _logger.LogInformation("PhotoSync: source and wwwroot/photos are the same directory — skipping file copy, updating DB only");

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var motorcycles = await db.Motorcycles.ToListAsync(cancellationToken);

            foreach (var modelDir in Directory.EnumerateDirectories(sourceRoot))
            {
                var folderName = Path.GetFileName(modelDir);

                // Match folder name to motorcycle name (case-insensitive, spaces/hyphens ignored)
                var motorcycle = motorcycles.FirstOrDefault(m =>
                    Normalize(m.Name) == Normalize(folderName));

                if (motorcycle is null)
                {
                    _logger.LogWarning("PhotoSync: no motorcycle matched folder '{Folder}'", folderName);
                    continue;
                }

                var destDir = Path.Combine(wwwPhotos, folderName);
                Directory.CreateDirectory(destDir);

                var imageFiles = Directory
                    .EnumerateFiles(modelDir)
                    .Where(f => IsImageFile(f))
                    .OrderBy(f => f)
                    .ToList();

                if (imageFiles.Count == 0) continue;

                if (!sameDir)
                {
                    foreach (var src in imageFiles)
                    {
                        var dest = Path.Combine(destDir, Path.GetFileName(src));
                        try
                        {
                            File.Copy(src, dest, overwrite: true);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "PhotoSync: skipping '{File}' — copy failed", Path.GetFileName(src));
                        }
                    }
                }

                // Replace DB image records for this motorcycle
                var existing = db.MotorcycleImages.Where(i => i.MotorcycleId == motorcycle.Id);
                db.MotorcycleImages.RemoveRange(existing);

                bool first = true;
                foreach (var src in imageFiles)
                {
                    var fileName = Path.GetFileName(src);
                    db.MotorcycleImages.Add(new MotorcycleImage
                    {
                        MotorcycleId = motorcycle.Id,
                        ImageUrl     = $"/photos/{folderName}/{fileName}",
                        IsMain       = first,
                    });
                    first = false;
                }

                _logger.LogInformation("PhotoSync: synced {Count} images for {Model}", imageFiles.Count, motorcycle.Name);
            }

            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "PhotoSync: startup sync failed — continuing without photos");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static string Normalize(string s) =>
        s.Replace(" ", "").Replace("-", "").ToUpperInvariant();

    private static bool IsImageFile(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png" or ".webp" or ".gif";
    }
}
