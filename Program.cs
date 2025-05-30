// Program.cs
using Microsoft.EntityFrameworkCore;
using ProjectBuildCraft.Data;       // for FragmentExporter
using ProjectBuildCraft.Models;
using ProjectBuildCraft.Services;

var builder = WebApplication.CreateBuilder(args);

// 1) Enable Razor Pages for Home/Privacy/About
// builder.Services.AddRazorPages();

// 2) Enable MVC controllers + views for your BuildController
builder.Services.AddControllersWithViews();

// 3) Register your DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=builds.db"));

// 4) Register your build-generator service
builder.Services.AddScoped<IBuildGeneratorService, BuildGeneratorService>();

// 5) Bind Bungie settings from appsettings.json into BungieOptions
builder.Services.Configure<BungieOptions>(
    builder.Configuration.GetSection("Bungie")
);

// 6) Register an HttpClient for all Bungie API calls
builder.Services.AddHttpClient("bungie", client =>
{
    client.BaseAddress = new Uri("https://www.bungie.net/");
    client.DefaultRequestHeaders.Add(
        "X-API-Key",
        builder.Configuration["Bungie:ApiKey"]
    );
});

// 7) Register the HTTP-backed ManifestService
builder.Services.AddSingleton<IManifestService, ManifestService>();

//One time exporter
// builder.Services.AddTransient<FragmentExporter>();
builder.Services.AddTransient<AspectExporter>();
builder.Services.AddSingleton<IAspectMetadataService, AspectMetadataService>();

builder.Configuration["ExoticMetadataPath"] = 
    Path.Combine(builder.Environment.ContentRootPath, "Data", "exotics-metadata.json");
builder.Services.AddTransient<ExoticExporter>();
builder.Services.AddSingleton<IExoticMetadataService, ExoticMetadataService>();


var app = builder.Build();

//
// ─── ONE-TIME FRAGMENT EXPORT ───────────────────────────────────────────────────
//
// This will write Data/fragments-raw-metadata.json for you to annotate.
// After you see the file on disk, comment out or remove this block.
//
// using (var scope = app.Services.CreateScope())
// {
//     var exporter = scope.ServiceProvider.GetRequiredService<FragmentExporter>();
//     // this will overwrite fragments-raw-metadata.json with a fresh set
//     exporter.ExportDefinitions(
//       Path.Combine(app.Environment.ContentRootPath,
//                    "Data",
//                    "fragments-raw-metadata.json")
//     );
//     // exit immediately so you see the dump
//     return;
// }

// In Program.cs, right before or after host.Build():
// var host = CreateHostBuilder(args).Build();

// ─── ONE-TIME ASPECT EXPORT ───────────────────────────────────────────────────


// using (var scope = app.Services.CreateScope())
// {
//     var exporter = scope.ServiceProvider.GetRequiredService<AspectExporter>();
//     var outPath  = Path.Combine(app.Environment.ContentRootPath, "Data", "aspects-raw-metadata.json");
//     Console.WriteLine($"[DEBUG] Writing aspects-raw-metadata.json to {outPath}");
//     exporter.ExportDefinitions(outPath);
//     Console.WriteLine($"[DEBUG] Done. Exit now.");
//     return;  // comment this back out after you inspect the file
// }

// app.Run();


// ── ONE-TIME EXOTIC EXPORT ───────────────────────────────────────────────────
// Writes Data/exotics-raw.json; comment out or remove after you inspect it.
// using (var scope = app.Services.CreateScope())
// {
//     var exporter = scope.ServiceProvider.GetRequiredService<ExoticExporter>();
//     var outPath  = Path.Combine(app.Environment.ContentRootPath,
//                                 "Data",
//                                 "exotics-raw.json");
//     exporter.ExportDefinitions(outPath);
//     Console.WriteLine($"Wrote raw exotics to: {outPath}");
//     return; // exit so you can open the JSON
// }

// app.Run();

//
// ─── EXISTING PRE-FETCH & MIGRATIONS ────────────────────────────────────────────
// (This will be skipped during the one-time export run)
//

using(var scope = app.Services.CreateScope())
{
    var manifestSvc = scope.ServiceProvider.GetRequiredService<IManifestService>();

    // warm armor per class
    foreach(var cls in new[] {
        GuardianClass.Titan,
        GuardianClass.Hunter,
        GuardianClass.Warlock
    })
    {
        manifestSvc.GetExoticArmor(cls);
    }

    // warm all weapons
    manifestSvc.GetExoticWeapons();

    // warm charge mods
    manifestSvc.GetChargeMods(0);
}


// 8) Auto-apply any pending migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 9) Map Razor Pages
// app.MapRazorPages();

// // 10) Map the Build controller
// app.MapControllerRoute(
//     name: "build",
//     pattern: "Build/{action=Index}/{id?}",
//     defaults: new { controller = "Build", action = "Index" }
// );
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Build}/{action=Index}/{id?}"
);


app.Run();
