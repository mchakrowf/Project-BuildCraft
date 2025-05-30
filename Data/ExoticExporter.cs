// Data/ExoticExporter.cs
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using ProjectBuildCraft.Models;
using ProjectBuildCraft.Services;

namespace ProjectBuildCraft.Data
{
    public class ExoticExporter
    {
        private readonly IManifestService _manifest;

        public ExoticExporter(IManifestService manifest)
            => _manifest = manifest;

        /// <summary>
        /// Exports every exotic armor and weapon’s hash, name, and (for weapons) element,
        /// de-duplicated by name, into two separate lists: Armor and Weapons.
        /// </summary>
        public void ExportDefinitions(string outputPath)
        {
            // 1) Pull every exotic armor for each class
            var allArmor = new[]
            {
                GuardianClass.Titan,
                GuardianClass.Hunter,
                GuardianClass.Warlock
            }
            .SelectMany(c => _manifest.GetExoticArmor(c))
            // collapse craftable vs normal duplicates by name
            .GroupBy(a => a.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .OrderBy(a => a.Name, StringComparer.OrdinalIgnoreCase)
            .Select(a => new {
                Hash = a.Hash,
                Name = a.Name
            })
            .ToList();

            // 2) Pull every exotic weapon
            var allWeapons = _manifest.GetExoticWeapons()
                // collapse craftable vs normal duplicates by name
                .GroupBy(w => w.Name.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .OrderBy(w => w.Name, StringComparer.OrdinalIgnoreCase)
                .Select(w => new {
                    Hash    = w.Hash,
                    Name    = w.Name,
                    Element = w.Element.ToString()
                })
                .ToList();

            // 3) Combine into a single anonymous with two lists
            var export = new {
                Armor   = allArmor,
                Weapons = allWeapons
            };

            // 4) Serialize & write
            var opts = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(export, opts);
            File.WriteAllText(outputPath, json);
        }
    }
}
