// Services/ManifestService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ProjectBuildCraft.Models;

namespace ProjectBuildCraft.Services
{
    public class ManifestService : IManifestService
    {
        private readonly HttpClient _http;
        private readonly BungieOptions _opts;

        // cache every InventoryItemDefinition (for fragments, aspects, charge mods)
        private Dictionary<long, InventoryItemDefinition>? _itemDefCache;
        // cache exotic cache for armor/weapon
        private List<ExoticItem>? _exoticCache;

        public ManifestService(IHttpClientFactory httpFactory,
                               IOptions<BungieOptions> opts)
        {
            _http = httpFactory.CreateClient("bungie");
            _opts = opts.Value;
        }

        private DestinyManifest GetManifest()
        {
            var resp = _http
                .GetFromJsonAsync<ApiResponse<DestinyManifest>>("Platform/Destiny2/Manifest/")
                .GetAwaiter().GetResult()!;
            return resp.Response;
        }

        private void EnsureItemDefCache()
        {
            if (_itemDefCache != null) return;

            var manifest = GetManifest();
            var path     = manifest.JsonWorldComponentContentPaths["en"]
                                    ["DestinyInventoryItemDefinition"];
            var url      = $"https://www.bungie.net{path}";
            var raw      = _http
                .GetFromJsonAsync<Dictionary<long, JsonElement>>(url)
                .GetAwaiter().GetResult() ?? new();

            _itemDefCache = raw
                .Select(kv => JsonSerializer
                    .Deserialize<InventoryItemDefinition>(kv.Value.GetRawText()))
                .Where(d => d is not null)
                .ToDictionary(d => d!.Hash, d => d!);
        }

        public IReadOnlyList<PerkDef> GetFragments()
        {
            EnsureItemDefCache();

            return _itemDefCache!.Values
                .Where(d =>
                    (d.Plug.PlugCategoryIdentifier
                         .IndexOf("fragment", StringComparison.OrdinalIgnoreCase) >= 0)
                 || (d.Plug.PlugCategoryIdentifier
                         .IndexOf("shard",    StringComparison.OrdinalIgnoreCase) >= 0)
                 || (d.Plug.PlugCategoryIdentifier
                         .IndexOf("stasis",   StringComparison.OrdinalIgnoreCase) >= 0))
                .Where(d => !d.Display.Name.StartsWith("Empty",
                                  StringComparison.OrdinalIgnoreCase))
                .Select(d => new PerkDef {
                    Hash        = d.Hash,
                    Name        = d.Display.Name,
                    IconPath    = d.Display.Icon,
                    Description = d.Display.Description,
                    FragmentSlots = 0
                })
                .ToList();
        }

        public IReadOnlyList<PerkDef> GetAspects()
        {
            EnsureItemDefCache();

            return _itemDefCache!.Values
                .Where(d =>
                    d.Plug.PlugCategoryIdentifier
                     .EndsWith(".aspects", StringComparison.OrdinalIgnoreCase)
                    && !d.Display.Name.StartsWith("Empty",
                          StringComparison.OrdinalIgnoreCase)
                )
                .Select(d => new PerkDef {
                    Hash        = d.Hash,
                    Name        = d.Display.Name,
                    IconPath    = d.Display.Icon,
                    Description = d.Display.Description,
                    FragmentSlots = 0
                })
                .ToList();
        }

        public IEnumerable<ExoticArmor> GetExoticArmor(GuardianClass @class)
        {
            EnsureExoticCache();

            return _exoticCache!
                .Where(e => e.ClassType == @class && e.IsArmor)
                .Select(e => new ExoticArmor {
                    Hash           = (int)e.Hash,
                    Id             = (int)e.Hash,
                    Name           = e.Display.Name,
                    IconPath       = e.Display.Icon,
                    Description    = e.Display.Description,
                    DestinyClassId = e.ClassTypeRaw
                });
        }

        public IEnumerable<ExoticWeapon> GetExoticWeapons()
        {
            EnsureExoticCache();

            return _exoticCache!
                .Where(e => !e.IsArmor)
                .Select(e => new ExoticWeapon {
                    Hash    = (int)e.Hash,
                    Name    = e.Display.Name,
                    Element = e.DefaultDamageType switch {
                        1 => ElementType.Arc,
                        2 => ElementType.Solar,
                        3 => ElementType.Void,
                        4 => ElementType.Stasis,
                        5 => ElementType.Strand,
                        _ => ElementType.Kinetic
                    }
                });
        }

        public IEnumerable<ModDef> GetChargeMods(int chargeTypeId)
        {
            EnsureItemDefCache();

            return _itemDefCache!.Values
                .Where(d => d.Hash.GetHashCode() == chargeTypeId)
                .Select(d => new ModDef {
                    Hash                    = d.Hash,
                    Name                    = d.Display.Name,
                    IconPath                = d.Display.Icon,
                    Description             = d.Display.Description,
                    PlugCategoryIdentifier  = d.Plug.PlugCategoryIdentifier
                });
        }

        public ModDef? GetModByName(string modName)
        {
            EnsureItemDefCache();

            var d = _itemDefCache!.Values
                       .FirstOrDefault(d => d.Display.Name == modName);
            if (d == null) return null;

            return new ModDef {
                Hash                   = d.Hash,
                Name                   = d.Display.Name,
                IconPath               = d.Display.Icon,
                Description            = d.Display.Description,
                PlugCategoryIdentifier = d.Plug.PlugCategoryIdentifier
            };
        }

        private void EnsureExoticCache()
        {
            if (_exoticCache != null) return;

            var manifest = GetManifest();
            var path     = manifest.JsonWorldComponentContentPaths["en"]
                                    ["DestinyInventoryItemDefinition"];
            var url      = $"https://www.bungie.net{path}";
            var raw      = _http
                .GetFromJsonAsync<Dictionary<long, JsonElement>>(url)
                .GetAwaiter().GetResult() ?? new();

            _exoticCache = raw
                .Select(kv => JsonSerializer.Deserialize<ExoticItem>(kv.Value.GetRawText()))
                .Where(e => e is not null && (e.IsArmor || e.IsWeapon))
                .Cast<ExoticItem>()
                .ToList();
        }
    }
}
