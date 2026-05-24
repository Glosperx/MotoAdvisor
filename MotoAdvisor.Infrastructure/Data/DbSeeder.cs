using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MotoAdvisor.Core.Entities;

namespace MotoAdvisor.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Always wipe and re-seed
        db.MotorcycleImages.RemoveRange(db.MotorcycleImages);
        db.UserFavorites.RemoveRange(db.UserFavorites);
        db.Reviews.RemoveRange(db.Reviews);
        db.Motorcycles.RemoveRange(db.Motorcycles);
        db.Brands.RemoveRange(db.Brands);
        db.Categories.RemoveRange(db.Categories);
        await db.SaveChangesAsync();

        // ── Brands ──────────────────────────────────────────────────────────
        var brands = new List<Brand>
        {
            new() { Name = "Honda",           Country = "Japan",     LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/38/Honda.svg/120px-Honda.svg.png" },
            new() { Name = "Yamaha",          Country = "Japan",     LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/47/Yamaha_Motor_logo.svg/120px-Yamaha_Motor_logo.svg.png" },
            new() { Name = "Kawasaki",        Country = "Japan",     LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/6/60/Kawasaki_motorcycles_logo.svg/120px-Kawasaki_motorcycles_logo.svg.png" },
            new() { Name = "Suzuki",          Country = "Japan",     LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/1/12/Suzuki_logo_2.svg/120px-Suzuki_logo_2.svg.png" },
            new() { Name = "Ducati",          Country = "Italy",     LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/8a/Ducati_red_logo.svg/120px-Ducati_red_logo.svg.png" },
            new() { Name = "BMW",             Country = "Germany",   LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/44/BMW.svg/120px-BMW.svg.png" },
            new() { Name = "KTM",             Country = "Austria",   LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/9/9b/KTM-Logo.svg/120px-KTM-Logo.svg.png" },
            new() { Name = "Triumph",         Country = "UK",        LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/87/Triumph_Motorcycles_logo.svg/120px-Triumph_Motorcycles_logo.svg.png" },
            new() { Name = "Harley-Davidson", Country = "USA",       LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/d/de/Harley-Davidson_logo.svg/120px-Harley-Davidson_logo.svg.png" },
            new() { Name = "Aprilia",         Country = "Italy",     LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/45/Aprilia-logo.svg/120px-Aprilia-logo.svg.png" },
            new() { Name = "CFMoto",          Country = "China",     LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c5/CFMoto_logo.svg/120px-CFMoto_logo.svg.png" },
        };
        db.Brands.AddRange(brands);

        // ── Categories ───────────────────────────────────────────────────────
        var cats = new List<Category>
        {
            new() { Name = "Sport",      Description = "High-performance bikes built for speed and precision on road and track." },
            new() { Name = "Supersport", Description = "Full-fairing track-focused machines with race-derived engines and aerodynamics." },
            new() { Name = "Naked",      Description = "Streetfighters with an upright riding position and minimal bodywork." },
            new() { Name = "Touring",    Description = "Comfortable long-distance motorcycles with luggage, wind protection and electronics." },
            new() { Name = "Adventure",  Description = "Versatile dual-sport bikes for on- and off-road exploration." },
            new() { Name = "Cruiser",    Description = "Low-slung, relaxed-posture bikes inspired by American highway culture." },
            new() { Name = "Scrambler",  Description = "Retro-styled bikes with off-road capability and urban attitude." },
            new() { Name = "Enduro",     Description = "Lightweight off-road-focused bikes built for dirt, gravel and trail riding." },
        };
        db.Categories.AddRange(cats);
        await db.SaveChangesAsync();

        // Shorthand refs
        var honda = brands[0]; var yamaha  = brands[1]; var kawasaki = brands[2];
        var suzuki = brands[3]; var ducati = brands[4]; var bmw      = brands[5];
        var ktm    = brands[6]; var triumph = brands[7]; var harley   = brands[8];
        var aprilia = brands[9]; var cfmoto = brands[10];

        var sport = cats[0]; var supersport = cats[1]; var naked   = cats[2];
        var touring = cats[3]; var adventure = cats[4]; var cruiser = cats[5];
        var scrambler = cats[6]; var enduro  = cats[7];

        static string Img(string name) =>
            $"https://placehold.co/600x400?text={Uri.EscapeDataString(name)}";

        // ── Motorcycles ──────────────────────────────────────────────────────
        var motos = new List<Motorcycle>
        {
            // ── HONDA (5) ──
            new() {
                Name = "CB500F", Year = 2024, Price = 6999, Engine = "471cc Parallel-Twin", Power = "47 hp",
                Horsepower = 47, LicenseCategory = "A2", IsBeginnerFriendly = true,
                Description = "The ideal first big bike. The CB500F's parallel-twin is smooth, forgiving and accessible, with a natural upright riding position that suits both commuting and weekend rides.",
                BrandId = honda.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "CB650R", Year = 2024, Price = 9499, Engine = "648.7cc Inline-4", Power = "95 hp",
                Horsepower = 95, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Neo Sports Café styling meets a silky four-cylinder engine. The CB650R bridges the gap between beginner-friendly nakeds and full-litre machines with usable, linear power.",
                BrandId = honda.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "CBR1000RR-R Fireblade", Year = 2024, Price = 28999, Engine = "999.9cc Inline-4", Power = "217 hp",
                Horsepower = 217, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Honda's MotoGP-derived superbike. Titanium connecting rods, finger-follower valve train and aerodynamic winglets deliver unmatched track performance.",
                BrandId = honda.Id, CategoryId = supersport.Id,
            },
            new() {
                Name = "Africa Twin CRF1100L", Year = 2024, Price = 15199, Engine = "1084cc Parallel-Twin", Power = "102 hp",
                Horsepower = 102, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The legend reborn. The Africa Twin's parallel-twin DCT option, long-travel suspension and lightweight chassis make it the benchmark adventure tourer for serious overlanders.",
                BrandId = honda.Id, CategoryId = adventure.Id,
            },
            new() {
                Name = "CB125R", Year = 2024, Price = 4599, Engine = "124.7cc Single", Power = "15 hp",
                Horsepower = 15, LicenseCategory = "A1", IsBeginnerFriendly = true,
                Description = "Premium mini-naked for A1 licence holders. Inverted forks, radial brakes and a steel trellis frame — the CB125R punches far above its licence category.",
                BrandId = honda.Id, CategoryId = naked.Id,
            },

            // ── YAMAHA (5) ──
            new() {
                Name = "YZF-R1", Year = 2024, Price = 20399, Engine = "998cc Crossplane Inline-4", Power = "200 hp",
                Horsepower = 200, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The crossplane crank delivers linear, tractable power that feels like a V4. MotoGP-derived electronics and aerodynamics make the R1 a razor-sharp track weapon.",
                BrandId = yamaha.Id, CategoryId = supersport.Id,
            },
            new() {
                Name = "MT-09", Year = 2024, Price = 10199, Engine = "889cc Inline-3", Power = "119 hp",
                Horsepower = 119, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The Dark Side of Japan. Yamaha's manic three-cylinder naked delivers explosive midrange torque, a flickable chassis and aggressive hyper-naked styling.",
                BrandId = yamaha.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "MT-07", Year = 2024, Price = 7999, Engine = "689cc Parallel-Twin", Power = "73 hp",
                Horsepower = 73, LicenseCategory = "A2", IsBeginnerFriendly = true,
                Description = "The world's best-selling naked. The MT-07's CP2 twin engine is characterful and torquey, its chassis forgiving, and its price tag impossible to argue with.",
                BrandId = yamaha.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "Ténéré 700", Year = 2024, Price = 11999, Engine = "689cc Parallel-Twin", Power = "72 hp",
                Horsepower = 72, LicenseCategory = "A2", IsBeginnerFriendly = false,
                Description = "Born from the Paris-Dakar legend. The Ténéré 700 is a pure adventure machine — light, capable off-road and utterly reliable across continents.",
                BrandId = yamaha.Id, CategoryId = adventure.Id,
            },
            new() {
                Name = "XSR700", Year = 2024, Price = 9499, Engine = "689cc Parallel-Twin", Power = "73 hp",
                Horsepower = 73, LicenseCategory = "A2", IsBeginnerFriendly = true,
                Description = "Retro-modern scrambler with MT-07 DNA. The XSR700 wraps Yamaha's best-selling twin in neo-retro styling with a beautifully balanced chassis.",
                BrandId = yamaha.Id, CategoryId = scrambler.Id,
            },

            // ── KAWASAKI (5) ──
            new() {
                Name = "Ninja ZX-10R", Year = 2024, Price = 17999, Engine = "998cc Inline-4", Power = "203 hp",
                Horsepower = 203, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Four-time Superbike World Championship platform. Race-bred cornering ABS, launch control and quickshifter transfer directly to the street.",
                BrandId = kawasaki.Id, CategoryId = supersport.Id,
            },
            new() {
                Name = "Z900", Year = 2024, Price = 10299, Engine = "948cc Inline-4", Power = "125 hp",
                Horsepower = 125, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Kawasaki's flagship naked. The Z900 combines aggressive Sugomi styling with a strong 948cc four, three riding modes and a price that undercuts rivals significantly.",
                BrandId = kawasaki.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "Z400", Year = 2024, Price = 5699, Engine = "399cc Parallel-Twin", Power = "44 hp",
                Horsepower = 44, LicenseCategory = "A2", IsBeginnerFriendly = true,
                Description = "A proper Kawasaki in miniature. The Z400's peppy twin, sport-tuned chassis and full-size feel make it the benchmark A2-category naked.",
                BrandId = kawasaki.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "Z125", Year = 2024, Price = 3499, Engine = "124.9cc Single", Power = "15 hp",
                Horsepower = 15, LicenseCategory = "A1", IsBeginnerFriendly = true,
                Description = "The entry-level Kawasaki Z with full-size attitude. Supermoto-inspired styling, inverted forks and a peppy single-cylinder engine perfect for city riding.",
                BrandId = kawasaki.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "Versys 1000 SE", Year = 2024, Price = 16499, Engine = "1043cc Inline-4", Power = "120 hp",
                Horsepower = 120, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Long-haul adventure touring with electronic suspension, cruise control and a smooth litre-class four. Continent-crossing made effortless.",
                BrandId = kawasaki.Id, CategoryId = adventure.Id,
            },

            // ── SUZUKI (4) ──
            new() {
                Name = "GSX-S1000", Year = 2024, Price = 13699, Engine = "999cc Inline-4", Power = "152 hp",
                Horsepower = 152, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Street-fighter attitude with GSX-R superbike DNA. Aggressive styling, comprehensive electronics and one of the best-sounding fours on the market.",
                BrandId = suzuki.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "GSX-8S", Year = 2024, Price = 8999, Engine = "776cc Parallel-Twin", Power = "83 hp",
                Horsepower = 83, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Suzuki's all-new middleweight naked with a punchy 776cc twin, modern electronics and a sport-touring riding position ideal for everyday use.",
                BrandId = suzuki.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "V-Strom 800DE", Year = 2024, Price = 11499, Engine = "776cc Parallel-Twin", Power = "84 hp",
                Horsepower = 84, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The do-it-all adventure bike. Lightweight, capable off-road and utterly practical on the highway, the V-Strom 800DE is Suzuki's most versatile machine.",
                BrandId = suzuki.Id, CategoryId = adventure.Id,
            },
            new() {
                Name = "Hayabusa", Year = 2024, Price = 18999, Engine = "1340cc Inline-4", Power = "190 hp",
                Horsepower = 190, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The icon that defined the sport-tourer class. The Hayabusa's muscular 1340cc engine, distinctive bodywork and advanced electronics are as compelling as ever.",
                BrandId = suzuki.Id, CategoryId = sport.Id,
            },

            // ── DUCATI (5) ──
            new() {
                Name = "Panigale V4 S", Year = 2024, Price = 36995, Engine = "1103cc Desmosedici Stradale V4", Power = "215 hp",
                Horsepower = 215, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "MotoGP for the road. Öhlins Smart EC 2.0 suspension, carbon wheels and Ducati's championship-winning V4 in its most exotic street form.",
                BrandId = ducati.Id, CategoryId = supersport.Id,
            },
            new() {
                Name = "Monster SP", Year = 2024, Price = 17495, Engine = "937cc Testastretta 11° V-Twin", Power = "111 hp",
                Horsepower = 111, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The Monster reinvented with a lightweight trellis frame. Fully adjustable Öhlins suspension and the thrilling V-Twin soundtrack define the SP experience.",
                BrandId = ducati.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "Multistrada V4 S", Year = 2024, Price = 29995, Engine = "1158cc V4 Granturismo", Power = "170 hp",
                Horsepower = 170, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Four bikes in one: sport, touring, urban, enduro. The Multistrada V4 S with radar-assisted adaptive cruise control is the world's most technologically advanced adventure tourer.",
                BrandId = ducati.Id, CategoryId = adventure.Id,
            },
            new() {
                Name = "Scrambler Icon", Year = 2024, Price = 10995, Engine = "803cc L-Twin", Power = "73 hp",
                Horsepower = 73, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The bike that revived the scrambler genre. Lightweight, beautiful and accessible, the Scrambler Icon is Ducati's most loveable machine.",
                BrandId = ducati.Id, CategoryId = scrambler.Id,
            },
            new() {
                Name = "DesertX", Year = 2024, Price = 16995, Engine = "937cc Testastretta 11° V-Twin", Power = "110 hp",
                Horsepower = 110, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Ducati's genuine off-road adventurer. Dakar-inspired styling, long-travel suspension and serious dirt capability wrapped around a beloved V-Twin.",
                BrandId = ducati.Id, CategoryId = adventure.Id,
            },

            // ── BMW (5) ──
            new() {
                Name = "R 1250 GS Adventure", Year = 2024, Price = 20850, Engine = "1254cc Boxer Twin", Power = "136 hp",
                Horsepower = 136, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The benchmark adventure motorcycle. The GS Adventure adds a larger fuel tank, longer suspension travel and more touring equipment to the world's most iconic adventure bike.",
                BrandId = bmw.Id, CategoryId = adventure.Id,
            },
            new() {
                Name = "S 1000 RR", Year = 2024, Price = 23450, Engine = "999cc Inline-4", Power = "210 hp",
                Horsepower = 210, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "BMW's MotoGP-developed superbike with ShiftCam variable valve timing, active aerodynamics and the most advanced electronics package in the class.",
                BrandId = bmw.Id, CategoryId = supersport.Id,
            },
            new() {
                Name = "F 900 R", Year = 2024, Price = 10100, Engine = "895cc Parallel-Twin", Power = "105 hp",
                Horsepower = 105, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "BMW's mid-class naked with a punchy parallel-twin, sharp ergonomics and a comprehensive electronics suite that belies its accessible price point.",
                BrandId = bmw.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "R 18", Year = 2024, Price = 19800, Engine = "1802cc Boxer Twin", Power = "91 hp",
                Horsepower = 91, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "BMW's grand American cruiser with the largest production boxer engine ever made. Torque-rich, beautifully engineered and dripping in chrome.",
                BrandId = bmw.Id, CategoryId = cruiser.Id,
            },
            new() {
                Name = "G 310 GS", Year = 2024, Price = 5500, Engine = "313cc Single", Power = "34 hp",
                Horsepower = 34, LicenseCategory = "A2", IsBeginnerFriendly = true,
                Description = "GS adventure heritage in an A2-legal package. The G 310 GS is BMW's entry point to the adventure segment — lightweight, nimble and genuinely capable off-road.",
                BrandId = bmw.Id, CategoryId = adventure.Id,
            },

            // ── KTM (5) ──
            new() {
                Name = "390 Duke", Year = 2024, Price = 5999, Engine = "399cc Single", Power = "44 hp",
                Horsepower = 44, LicenseCategory = "A2", IsBeginnerFriendly = true,
                Description = "The definitive A2 naked. The 390 Duke's punchy single, trellis chassis and full-colour TFT display deliver a premium experience at an entry-level price.",
                BrandId = ktm.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "890 Duke R", Year = 2024, Price = 12499, Engine = "889cc Parallel-Twin", Power = "121 hp",
                Horsepower = 121, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The Super Scalpel. KTM's middleweight twin with WP Apex suspension, corner-by-corner ABS and launch control in a featherweight naked package.",
                BrandId = ktm.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "1290 Super Duke R EVO", Year = 2024, Price = 21999, Engine = "1301cc V-Twin", Power = "180 hp",
                Horsepower = 180, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The Beast, evolved. Semi-active WP suspension, radar-assisted cruise and 180 hp from the ferocious V-Twin make the 1290 the most extreme naked money can buy.",
                BrandId = ktm.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "790 Adventure R", Year = 2024, Price = 13499, Engine = "799cc Parallel-Twin", Power = "95 hp",
                Horsepower = 95, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Rally-bred off-road adventure. WP XPLOR suspension, 21/18 spoke wheels and rally-style ergonomics make the 790 Adventure R a serious dirt machine.",
                BrandId = ktm.Id, CategoryId = adventure.Id,
            },
            new() {
                Name = "500 EXC-F", Year = 2024, Price = 11999, Engine = "510cc Single", Power = "63 hp",
                Horsepower = 63, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The enduro benchmark. Lightweight, powerful and extremely capable on trails, the 500 EXC-F is street-legal off-road excellence from KTM's race department.",
                BrandId = ktm.Id, CategoryId = enduro.Id,
            },

            // ── TRIUMPH (4) ──
            new() {
                Name = "Street Triple RS", Year = 2024, Price = 12995, Engine = "765cc Inline-3", Power = "130 hp",
                Horsepower = 130, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The triple-cylinder benchmark. The Street Triple RS's Moto2-derived engine, Öhlins suspension and Brembo brakes make it the finest middleweight naked available.",
                BrandId = triumph.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "Tiger 900 Rally Pro", Year = 2024, Price = 16295, Engine = "888cc Inline-3", Power = "95 hp",
                Horsepower = 95, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Triumph's rally-inspired adventure tourer. Showa semi-active suspension, cornering ABS and a torquey triple make it equally at home on gravel and tarmac.",
                BrandId = triumph.Id, CategoryId = adventure.Id,
            },
            new() {
                Name = "Bonneville T120", Year = 2024, Price = 13100, Engine = "1200cc Parallel-Twin", Power = "79 hp",
                Horsepower = 79, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The icon that defined British motorcycling. The modern T120 blends classic 1959 styling with fuel injection, ride-by-wire and traction control.",
                BrandId = triumph.Id, CategoryId = scrambler.Id,
            },
            new() {
                Name = "Trident 660", Year = 2024, Price = 8295, Engine = "660cc Inline-3", Power = "81 hp",
                Horsepower = 81, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Triumph's accessible triple. Light, agile and characterful, the Trident 660 brings the brand's triple-cylinder DNA to an attainable, everyday naked.",
                BrandId = triumph.Id, CategoryId = naked.Id,
            },

            // ── HARLEY-DAVIDSON (4) ──
            new() {
                Name = "Sportster S", Year = 2024, Price = 16499, Engine = "1252cc Revolution Max V-Twin", Power = "121 hp",
                Horsepower = 121, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Harley's high-performance liquid-cooled Sportster. The Revolution Max engine delivers 94 lb-ft of torque in a low-slung, aggressive cruiser package.",
                BrandId = harley.Id, CategoryId = cruiser.Id,
            },
            new() {
                Name = "Road Glide ST", Year = 2024, Price = 36999, Engine = "1923cc Milwaukee-Eight 117 V-Twin", Power = "100 hp",
                Horsepower = 100, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The ultimate American touring machine. Shark-nose frame-mounted fairing, BOOM! Box infotainment and 117 cubic inches of Milwaukee thunder.",
                BrandId = harley.Id, CategoryId = touring.Id,
            },
            new() {
                Name = "Fat Bob 114", Year = 2024, Price = 20999, Engine = "1868cc Milwaukee-Eight 114 V-Twin", Power = "93 hp",
                Horsepower = 93, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Dark, muscular and rebellious. The Fat Bob's aggressive stance, wide rear tyre and Milwaukee-Eight rumble make it Harley's most visceral everyday cruiser.",
                BrandId = harley.Id, CategoryId = cruiser.Id,
            },
            new() {
                Name = "Pan America 1250 Special", Year = 2024, Price = 23999, Engine = "1252cc Revolution Max V-Twin", Power = "150 hp",
                Horsepower = 150, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Harley's first serious adventure bike. Adaptive ride height, semi-active suspension and 150 hp from the Revolution Max engine prove Harley can do ADV.",
                BrandId = harley.Id, CategoryId = adventure.Id,
            },

            // ── APRILIA (4) ──
            new() {
                Name = "RSV4 Factory", Year = 2024, Price = 29999, Engine = "1099cc V4", Power = "217 hp",
                Horsepower = 217, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The most powerful V4 superbike in production. Seven-time Superbike World Championship winner with APRC electronics, Öhlins Smart EC and aerodynamic winglets.",
                BrandId = aprilia.Id, CategoryId = supersport.Id,
            },
            new() {
                Name = "Tuono V4 Factory", Year = 2024, Price = 27499, Engine = "1099cc V4", Power = "175 hp",
                Horsepower = 175, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "The world's most powerful naked. Sharing its V4 engine with the RSV4, the Tuono delivers supercar performance with a more comfortable, upright riding position.",
                BrandId = aprilia.Id, CategoryId = naked.Id,
            },
            new() {
                Name = "RS 457", Year = 2024, Price = 6499, Engine = "457cc Parallel-Twin", Power = "47 hp",
                Horsepower = 47, LicenseCategory = "A2", IsBeginnerFriendly = true,
                Description = "A genuine supersport for A2 licence holders. The RS 457's full-fairing bodywork, four-cylinder-style chassis and 47 hp twin make it the most exciting A2 bike available.",
                BrandId = aprilia.Id, CategoryId = supersport.Id,
            },
            new() {
                Name = "Tuareg 660", Year = 2024, Price = 13499, Engine = "659cc Parallel-Twin", Power = "80 hp",
                Horsepower = 80, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Aprilia's rally-bred adventure machine. 21-inch front wheel, long-travel suspension and an 80 hp twin purpose-built for desert exploration.",
                BrandId = aprilia.Id, CategoryId = adventure.Id,
            },

            // ── CFMOTO (4) ──
            new() {
                Name = "700CL-X Heritage", Year = 2024, Price = 7499, Engine = "693cc Parallel-Twin", Power = "70 hp",
                Horsepower = 70, LicenseCategory = "A2", IsBeginnerFriendly = true,
                Description = "A2-legal scrambler with a premium build quality that punches well above its price. The 700CL-X Heritage's retro styling and KTM-supplied engine make it a serious bargain.",
                BrandId = cfmoto.Id, CategoryId = scrambler.Id,
            },
            new() {
                Name = "450SR", Year = 2024, Price = 5299, Engine = "449.4cc Parallel-Twin", Power = "47 hp",
                Horsepower = 47, LicenseCategory = "A2", IsBeginnerFriendly = true,
                Description = "CFMoto's A2 supersport brings full-fairing styling and a rev-happy twin to licence-restricted riders at a genuinely competitive price point.",
                BrandId = cfmoto.Id, CategoryId = supersport.Id,
            },
            new() {
                Name = "800MT Explore", Year = 2024, Price = 9799, Engine = "799cc Parallel-Twin", Power = "95 hp",
                Horsepower = 95, LicenseCategory = "A", IsBeginnerFriendly = false,
                Description = "Mid-size adventure tourer with KTM-derived engine, a full electronics package and adventure-ready ergonomics at a price that undercuts the European competition by thousands.",
                BrandId = cfmoto.Id, CategoryId = adventure.Id,
            },
            new() {
                Name = "300NK", Year = 2024, Price = 3299, Engine = "292cc Single", Power = "28 hp",
                Horsepower = 28, LicenseCategory = "A2", IsBeginnerFriendly = true,
                Description = "Entry-level urban naked at an unbeatable price. The 300NK's single-cylinder engine, modern LED lighting and smartphone connectivity make it an ideal city commuter.",
                BrandId = cfmoto.Id, CategoryId = naked.Id,
            },
        };

        foreach (var moto in motos)
        {
            db.Motorcycles.Add(moto);
            await db.SaveChangesAsync();

            db.MotorcycleImages.Add(new MotorcycleImage
            {
                MotorcycleId = moto.Id,
                ImageUrl     = Img(moto.Name),
                IsMain       = true,
            });
        }

        await db.SaveChangesAsync();

        // ── Admin user ───────────────────────────────────────────────────────
        const string adminEmail = "admin@motoadvisor.com";
        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing != null) await userManager.DeleteAsync(existing);

        var admin = new ApplicationUser
        {
            UserName       = "admin",
            Email          = adminEmail,
            EmailConfirmed = true,
        };
        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
    }
}
