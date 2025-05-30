// Controllers/BuildController.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectBuildCraft.Data;
using ProjectBuildCraft.Models;
using ProjectBuildCraft.Services;
using ProjectBuildCraft.ViewModels;

namespace ProjectBuildCraft.Controllers
{
    public class BuildController : Controller
    {
        private readonly AppDbContext            _db;
        private readonly IWebHostEnvironment     _env;
        private readonly IManifestService        _manifest;
        private readonly IBuildGeneratorService  _buildGenerator;
        private static readonly string[]         _imgExts = { ".png", ".jpg", ".jpeg", ".gif" };

        public BuildController(
            AppDbContext db,
            IWebHostEnvironment env,
            IManifestService manifestService,
            IBuildGeneratorService buildGeneratorService)
        {
            _db             = db;
            _env            = env;
            _manifest       = manifestService;
            _buildGenerator = buildGeneratorService;
        }

        [HttpGet("/Build/DebugPerks")]
        public IActionResult DebugPerks()
        {
            var aspects   = _manifest.GetAspects();
            var fragments = _manifest.GetFragments();
            return Json(new {
                TotalPerks      = aspects.Count + fragments.Count,
                AspectCount     = aspects.Count,
                FragmentCount   = fragments.Count,
                SampleAspects   = aspects.Take(10).Select(p => new { p.Hash, p.Name }),
                SampleFragments = fragments.Take(10).Select(p => new { p.Hash, p.Name })
            });
        }

        private static GuardianClass MapClassNameToManifestClass(string dbClassName) =>
            dbClassName switch {
                "Titan"   => GuardianClass.Titan,
                "Hunter"  => GuardianClass.Hunter,
                "Warlock" => GuardianClass.Warlock,
                _ => throw new ArgumentOutOfRangeException(nameof(dbClassName),
                        $"Unknown class '{dbClassName}'")
            };

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
            var vm = new BuildViewModel {
                Classes         = await LoadClassItemsAsync(),
                Subclasses      = new List<SelectItemWithImage>(),
                Armors          = new List<SelectListItem>(),
                Weapons         = await LoadWeaponsAsync(),
                FocusOptions    = await LoadFocusOptionsAsync(),
                ChargeTypes     = await LoadChargeTypesAsync(),
                Recommendation  = new BuildMapping(),
                ModLoadout      = new Dictionary<string, List<ModDef>>(),
                AspectLoadout   = new Dictionary<string, List<Aspect>>(),
                FragmentLoadout = new Dictionary<string, List<Fragment>>()
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BuildViewModel vm)
        {
            vm.Classes      = await LoadClassItemsAsync();
            vm.Subclasses   = new List<SelectItemWithImage>();
            vm.Armors       = vm.SelectedClassId.HasValue
                                ? await LoadArmorsAsync(vm.SelectedClassId.Value)
                                : new List<SelectListItem>();
            vm.Weapons      = await LoadWeaponsAsync();
            vm.FocusOptions = await LoadFocusOptionsAsync();
            vm.ChargeTypes  = await LoadChargeTypesAsync();

            if (vm.SelectedClassId.HasValue
            && vm.SelectedSubclassId.HasValue
            && vm.SelectedArmorId.HasValue
            && vm.SelectedWeaponHash.HasValue
            && vm.SelectedFocusId.HasValue
            && vm.SelectedChargeTypeId.HasValue)
            {
                var dbClass = await _db.Classes.FindAsync(vm.SelectedClassId.Value);
                if (dbClass == null)
                {
                    ModelState.AddModelError(
                        nameof(vm.SelectedClassId),
                        "The selected class was not found."
                    );
                    return View(vm);
                }

                var seeded = await _db.Mappings
                    .Include(m => m.Aspects)
                    .Include(m => m.Fragments)
                    .FirstOrDefaultAsync(m =>
                        m.ClassId       == vm.SelectedClassId.Value &&
                        m.SubclassId    == vm.SelectedSubclassId.Value &&
                        m.ArmorId       == vm.SelectedArmorId.Value &&
                        m.WeaponId      == vm.SelectedWeaponHash.Value &&
                        m.FocusOptionId == vm.SelectedFocusId.Value &&
                        m.ChargeTypeId  == vm.SelectedChargeTypeId.Value
                    );

                if (seeded != null)
                {
                    vm.Recommendation  = seeded;
                    vm.ModLoadout      = ParseModLoadout(seeded);
                    vm.AspectLoadout   = seeded.Aspects
                                                 .ToLookup(a => "Aspects", a => a)
                                                 .ToDictionary(g => g.Key, g => g.ToList());
                    vm.FragmentLoadout = seeded.Fragments
                                                 .ToLookup(f => "Fragments", f => f)
                                                 .ToDictionary(g => g.Key, g => g.ToList());
                }
                else
                {
                    var dynamicRec = _buildGenerator.GenerateBuild(
                        MapClassNameToManifestClass(dbClass.Name),
                        vm.SelectedSubclassId.Value,
                        vm.SelectedArmorId.Value,
                        vm.SelectedWeaponHash.Value,
                        vm.SelectedFocusId.Value,
                        vm.SelectedChargeTypeId.Value
                    );

                    vm.Recommendation   = dynamicRec;
                    vm.ModLoadout       = ParseModLoadout(dynamicRec);
                    vm.AspectLoadout    = dynamicRec.Aspects
                                                    .ToLookup(a => "Aspects", a => a)
                                                    .ToDictionary(g => g.Key, g => g.ToList());
                    vm.FragmentLoadout  = dynamicRec.Fragments
                                                    .ToLookup(f => "Fragments", f => f)
                                                    .ToDictionary(g => g.Key, g => g.ToList());
                }
            }
            else
            {
                vm.Recommendation = new BuildMapping {
                    Summary = "Please complete all selections",
                    Mods    = string.Empty
                };
            }

            return View(vm);
        }

        private Dictionary<string, List<ModDef>> ParseModLoadout(BuildMapping m) =>
            m.Mods
             .Split(';', StringSplitOptions.RemoveEmptyEntries)
             .Select(token => token.Trim().Split(':', 2))
             .Where(parts => parts.Length == 2)
             .Select(parts => (slot: parts[0], name: parts[1]))
             .ToLookup(x => x.slot, x => _manifest.GetModByName(x.name)!)
             .ToDictionary(g => g.Key, g => g.ToList());

        [HttpGet]
        public async Task<JsonResult> GetSubclasses(int classId)
        {
            var subs    = await _db.Subclasses.Where(s => s.DestinyClassId == classId).ToListAsync();
            var clsName = (await _db.Classes.FindAsync(classId))?.Name ?? "";
            var dir     = Path.Combine(_env.WebRootPath, "images", "subclasses");

            var data = subs.Select(s => {
                var tries = new[] { s.Name, s.Name.Replace(" ", ""), s.Name + clsName };
                var img   = tries
                    .Select(t => FindImagePath(dir, t))
                    .FirstOrDefault(p => p != null)
                    ?? Path.Combine(dir, "default.png");
                return new {
                    id       = s.Id,
                    name     = s.Name,
                    imageUrl = Url.Content($"~/images/subclasses/{Path.GetFileName(img)}")
                };
            });

            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetArmors(int classId)
        {
            var dbClass = await _db.Classes.FindAsync(classId);
            if (dbClass == null)
                return Json(Array.Empty<object>());

            var manClass = MapClassNameToManifestClass(dbClass.Name);
            var armors = _manifest.GetExoticArmor(manClass)
                .GroupBy(a => a.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
                .Select(a => new { id = a.Hash, name = a.Name });
            return Json(armors);
        }

        private async Task<List<SelectItemWithImage>> LoadClassItemsAsync()
        {
            var classes = await _db.Classes.Where(c => c.Id >= 1 && c.Id <= 3).ToListAsync();
            return classes.Select(c => {
                var img = FindImagePath(
                    Path.Combine(_env.WebRootPath, "images", "classes"),
                    c.Name
                ) ?? Path.Combine(_env.WebRootPath, "images", "classes", "default.png");

                return new SelectItemWithImage {
                    Id       = c.Id.ToString(),
                    Name     = c.Name,
                    ImageUrl = Url.Content($"~/images/classes/{Path.GetFileName(img)}")
                };
            }).ToList();
        }

        private async Task<List<SelectListItem>> LoadArmorsAsync(int classId)
        {
            var dbClass = await _db.Classes.FindAsync(classId);
            if (dbClass == null) return new();

            var manClass = MapClassNameToManifestClass(dbClass.Name);
            return _manifest
                .GetExoticArmor(manClass)
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem(a.Name, a.Hash.ToString()))
                .ToList();
        }

        private Task<List<SelectListItem>> LoadWeaponsAsync()
        {
            var items = _manifest
                .GetExoticWeapons()
                .GroupBy(w => w.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .OrderBy(w => w.Name, StringComparer.OrdinalIgnoreCase)
                .Select(w => new SelectListItem(w.Name, w.Hash.ToString()))
                .ToList();

            return Task.FromResult(items);
        }

        private Task<List<SelectListItem>> LoadFocusOptionsAsync() =>
            _db.FocusOptions
               .Select(f => new SelectListItem(f.Name, f.Id.ToString()))
               .ToListAsync();

        private Task<List<SelectListItem>> LoadChargeTypesAsync() =>
            _db.ChargeTypes
               .Select(ct => new SelectListItem(ct.Name, ct.Id.ToString()))
               .ToListAsync();
    }
}
