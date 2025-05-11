using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectBuildCraft.Data;
using ProjectBuildCraft.Models;       // for BuildMapping
using ProjectBuildCraft.ViewModels;   // for SelectItemWithImage & BuildViewModel

namespace ProjectBuildCraft.Controllers
{
    public class BuildController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;
        private static readonly string[] _imgExts = { ".png", ".jpg", ".jpeg", ".gif" };

        public BuildController(AppDbContext db, IWebHostEnvironment env)
        {
            _db  = db;
            _env = env;
        }

        // Helper: find the first matching image file by baseName
        private string? FindImagePath(string directory, string baseName)
        {
            foreach (var ext in _imgExts)
            {
                var candidate = Path.Combine(directory, baseName + ext);
                if (System.IO.File.Exists(candidate))
                    return candidate;
            }
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // 1) Load Classes as image-based items
            var classItems = (await _db.Classes.ToListAsync())
                .Select(c => {
                    var img = FindImagePath(
                        Path.Combine(_env.WebRootPath, "images", "classes"),
                        c.Name
                    ) ?? Path.Combine(_env.WebRootPath, "images", "classes", "default.png");
                    return new SelectItemWithImage {
                        Id       = c.Id.ToString(),
                        Name     = c.Name,
                        ImageUrl = Url.Content($"~/images/classes/{Path.GetFileName(img)}")
                    };
                })
                .ToList();

            // 2) Build VM with image-Classes + empty image-Subclasses
            //    and standard dropdowns for everything else
            var vm = new BuildViewModel
            {
                Classes     = classItems,
                Subclasses  = new List<SelectItemWithImage>(),
                Armors      = new List<SelectListItem>(),
                Weapons     = await _db.Weapons
                                     .Select(w => new SelectListItem(w.Name, w.Id.ToString()))
                                     .ToListAsync(),
                Stats       = await _db.Stats
                                     .Select(s => new SelectListItem(s.Name, s.Id.ToString()))
                                     .ToListAsync(),
                FocusOptions= await _db.FocusOptions
                                     .Select(f => new SelectListItem(f.Name, f.Id.ToString()))
                                     .ToListAsync()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BuildViewModel vm)
        {
            // 1) Repopulate Classes
            vm.Classes = (await _db.Classes.ToListAsync())
                .Select(c => {
                    var img = FindImagePath(
                        Path.Combine(_env.WebRootPath, "images", "classes"),
                        c.Name
                    ) ?? Path.Combine(_env.WebRootPath, "images", "classes", "default.png");
                    return new SelectItemWithImage {
                        Id       = c.Id.ToString(),
                        Name     = c.Name,
                        ImageUrl = Url.Content($"~/images/classes/{Path.GetFileName(img)}")
                    };
                })
                .ToList();

            // 2) Subclasses will be fetched via AJAX
            vm.Subclasses = new List<SelectItemWithImage>();

            // 3) Repopulate Armors (standard dropdown)
            vm.Armors = vm.SelectedClassId.HasValue
                ? await _db.Armors
                      .Where(a => a.DestinyClassId == vm.SelectedClassId.Value)
                      .Select(a => new SelectListItem(a.Name, a.Id.ToString()))
                      .ToListAsync()
                : new List<SelectListItem>();

            // 4) Repopulate Weapons, Stats & Focus (standard dropdowns)
            vm.Weapons     = await _db.Weapons
                                   .Select(w => new SelectListItem(w.Name, w.Id.ToString()))
                                   .ToListAsync();
            vm.Stats       = await _db.Stats
                                   .Select(s => new SelectListItem(s.Name, s.Id.ToString()))
                                   .ToListAsync();
            vm.FocusOptions= await _db.FocusOptions
                                   .Select(f => new SelectListItem(f.Name, f.Id.ToString()))
                                   .ToListAsync();

            // 5) Recommendation logic
            var mapping = vm.SelectedClassId.HasValue
                       && vm.SelectedSubclassId.HasValue
                       && vm.SelectedArmorId.HasValue
                       && vm.SelectedWeaponId.HasValue
                       && vm.SelectedFocusId.HasValue
                       && vm.PrimaryStatId.HasValue
                       && vm.SecondaryStatId.HasValue
                ? await _db.Mappings.FirstOrDefaultAsync(m =>
                      m.ClassId         == vm.SelectedClassId.Value &&
                      m.SubclassId      == vm.SelectedSubclassId.Value &&
                      m.ArmorId         == vm.SelectedArmorId.Value &&
                      m.WeaponId        == vm.SelectedWeaponId.Value &&
                      m.FocusOptionId   == vm.SelectedFocusId.Value &&
                      m.PrimaryStatId   == vm.PrimaryStatId.Value &&
                      m.SecondaryStatId == vm.SecondaryStatId.Value)
                : null;

            vm.Recommendation = mapping ?? new BuildMapping {
                Summary = "No matching build found",
                Mods    = "Try tweaking one of your selections!"
            };

            return View(vm);
        }

        // --- AJAX endpoints ---

        // Returns { id, name, imageUrl } for subclasses of a class
        [HttpGet]
        public async Task<JsonResult> GetSubclasses(int classId)
        {
            var subs = await _db.Subclasses
                .Where(s => s.DestinyClassId == classId)
                .ToListAsync();

            // look up class name for fallback
            var cls       = await _db.Classes.FindAsync(classId);
            var className = cls?.Name ?? "";

            var dir = Path.Combine(_env.WebRootPath, "images", "subclasses");

            var data = subs.Select(s => {
                // try plain name, then name+class
                var plainPath = FindImagePath(dir, s.Name);
                var comboPath = FindImagePath(dir, s.Name + className);
                var finalPath = plainPath
                              ?? comboPath
                              ?? Path.Combine(dir, "default.png");
                return new {
                    id       = s.Id,
                    name     = s.Name,
                    imageUrl = Url.Content($"~/images/subclasses/{Path.GetFileName(finalPath)}")
                };
            });

            return Json(data);
        }

        // Returns { id, name } for armors (standard dropdown)
        [HttpGet]
        public async Task<JsonResult> GetArmors(int classId) =>
            Json(await _db.Armors
                .Where(a => a.DestinyClassId == classId)
                .Select(a => new { id = a.Id, name = a.Name })
                .ToListAsync());

        // Returns { id, name } for weapons (if needed dynamically)
        [HttpGet]
        public async Task<JsonResult> GetWeapons() =>
            Json(await _db.Weapons
                .Select(w => new { id = w.Id, name = w.Name })
                .ToListAsync());
    }
}
