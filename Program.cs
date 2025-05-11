// Program.cs
using Microsoft.EntityFrameworkCore;
using ProjectBuildCraft.Data;

var builder = WebApplication.CreateBuilder(args);

// 1) Enable Razor Pages for Home/Privacy/About
builder.Services.AddRazorPages();

// 2) Enable MVC controllers + views for your BuildController
builder.Services.AddControllersWithViews();

// 3) Register your DbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=builds.db"));

var app = builder.Build();

// 4) Auto‐apply any pending migrations
using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
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

// 5) First map your Razor Pages (so “/”, “/Privacy”, “/About” work)
app.MapRazorPages();

// 6) Then map only the Build controller under /Build
app.MapControllerRoute(
    name: "build",
    pattern: "Build/{action=Index}/{id?}",
    defaults: new { controller = "Build", action = "Index" }
);

app.Run();
