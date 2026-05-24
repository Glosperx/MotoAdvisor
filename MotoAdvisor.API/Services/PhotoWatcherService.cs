using System.Threading.Channels;
using Microsoft.EntityFrameworkCore;
using MotoAdvisor.Core.Entities;
using MotoAdvisor.Infrastructure.Data;

namespace MotoAdvisor.API.Services;

/// <summary>
/// Watches ~/Documents/MotoAdvisor/Photos/ for new images at runtime.
/// When an image lands in a model subfolder it is copied to wwwroot and
/// a MotorcycleImage DB record is inserted — no restart needed.
/// </summary>
public class PhotoWatcherService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IWebHostEnvironment  _env;
    private readonly IConfiguration       _config;
    private readonly ILogger<PhotoWatcherService> _logger;

    // Queue ensures watcher callbacks (thread-pool threads) hand off safely to
    // the single async consumer. Bounded so it never grows unbounded.
    private readonly Channel<string> _queue = Channel.CreateBounded<string>(
        new BoundedChannelOptions(512)
        {
            SingleReader  = true,
            FullMode      = BoundedChannelFullMode.DropOldest,
        });

    private string _sourceRoot = string.Empty;

    public PhotoWatcherService(
        IServiceScopeFactory scopeFactory,
        IWebHostEnvironment  env,
        IConfiguration       config,
        ILogger<PhotoWatcherService> logger)
    {
        _scopeFactory = scopeFactory;
        _env          = env;
        _config       = config;
        _logger       = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _sourceRoot = (_config["Photos:SourcePath"] ?? string.Empty)
                .Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

            if (!Directory.Exists(_sourceRoot))
            {
                _logger.LogWarning("PhotoWatcher: source directory not found – {Path}", _sourceRoot);
                return;
            }

            using var watcher = new FileSystemWatcher(_sourceRoot)
            {
                IncludeSubdirectories = true,
                NotifyFilter          = NotifyFilters.FileName,
                EnableRaisingEvents   = true,
            };

            watcher.Created += (_, e) => TryEnqueue(e.FullPath);
            // Renamed covers drag-drop and mv-into-folder scenarios
            watcher.Renamed += (_, e) => TryEnqueue(e.FullPath);

            _logger.LogInformation("PhotoWatcher: watching {Path}", _sourceRoot);

            await foreach (var filePath in _queue.Reader.ReadAllAsync(stoppingToken))
            {
                await ProcessFileAsync(filePath, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "PhotoWatcher: watcher failed — live photo sync disabled");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _queue.Writer.Complete();
        await base.StopAsync(cancellationToken);
    }

    // ── Watcher callback (thread-pool) ──────────────────────────────────────

    private void TryEnqueue(string path)
    {
        if (IsImageFile(path) && IsDirectChild(path))
            _queue.Writer.TryWrite(path);
    }

    // Only handle files exactly one level deep: Photos/{Model}/{file}
    private bool IsDirectChild(string path)
    {
        var rel = Path.GetRelativePath(_sourceRoot, path);
        return rel.Split(Path.DirectorySeparatorChar).Length == 2;
    }

    // ── Consumer (single async loop) ────────────────────────────────────────

    private async Task ProcessFileAsync(string srcPath, CancellationToken ct)
    {
        // Wait for the writing process to release the file before copying.
        // Retries up to 5 × 400 ms = 2 s.
        if (!await WaitUntilReadyAsync(srcPath, ct))
        {
            _logger.LogWarning("PhotoWatcher: timed out waiting for file to be readable – {Path}", srcPath);
            return;
        }

        var folderName = Path.GetFileName(Path.GetDirectoryName(srcPath)!);
        var fileName   = Path.GetFileName(srcPath);
        var imageUrl   = $"/photos/{folderName}/{fileName}";
        var destDir    = Path.Combine(_env.WebRootPath, "photos", folderName);
        var destPath   = Path.Combine(destDir, fileName);

        try
        {
            Directory.CreateDirectory(destDir);
            File.Copy(srcPath, destPath, overwrite: true);
        }
        catch (IOException ex)
        {
            _logger.LogError(ex, "PhotoWatcher: copy failed for {Path}", srcPath);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Match folder name to motorcycle (normalise both sides)
        var motorcycles = await db.Motorcycles.ToListAsync(ct);
        var motorcycle  = motorcycles.FirstOrDefault(m =>
            Normalize(m.Name) == Normalize(folderName));

        if (motorcycle is null)
        {
            _logger.LogWarning("PhotoWatcher: no motorcycle matched folder '{Folder}'", folderName);
            return;
        }

        // Idempotency check – don't insert duplicates
        var exists = await db.MotorcycleImages
            .AnyAsync(i => i.MotorcycleId == motorcycle.Id && i.ImageUrl == imageUrl, ct);
        if (exists)
        {
            _logger.LogDebug("PhotoWatcher: image already recorded – {Url}", imageUrl);
            return;
        }

        // Make it main only when the motorcycle has no images yet
        var hasAny = await db.MotorcycleImages
            .AnyAsync(i => i.MotorcycleId == motorcycle.Id, ct);

        db.MotorcycleImages.Add(new MotorcycleImage
        {
            MotorcycleId = motorcycle.Id,
            ImageUrl     = imageUrl,
            IsMain       = !hasAny,
        });

        await db.SaveChangesAsync(ct);
        _logger.LogInformation("PhotoWatcher: added {Url} → {Model}", imageUrl, motorcycle.Name);
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    private static async Task<bool> WaitUntilReadyAsync(string path, CancellationToken ct)
    {
        for (int attempt = 0; attempt < 5; attempt++)
        {
            await Task.Delay(400, ct);

            if (!File.Exists(path)) return false;

            try
            {
                using var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None);
                return true;
            }
            catch (IOException) { /* file still locked – retry */ }
        }
        return false;
    }

    private static string Normalize(string s) =>
        s.Replace(" ", "").Replace("-", "").ToUpperInvariant();

    private static bool IsImageFile(string path)
    {
        var ext = Path.GetExtension(path).ToLowerInvariant();
        return ext is ".jpg" or ".jpeg" or ".png" or ".webp" or ".gif";
    }
}
