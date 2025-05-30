// ProjectBuildCraft/Data/FragmentExporter.cs
using System.IO;
using System.Linq;
using System.Text.Json;
using ProjectBuildCraft.Models;    // for PerkDef
using ProjectBuildCraft.Services;  // for IManifestService

namespace ProjectBuildCraft.Data
{
    public class FragmentExporter
    {
        private readonly IManifestService _manifest;
        public FragmentExporter(IManifestService manifest)
            => _manifest = manifest;

        /// <summary>
        /// Exports every fragment’s hash, name, and description,
        /// plus placeholder fields for your metadata.
        /// </summary>
        public void ExportDefinitions(string outputPath)
        {
            var fragments = _manifest.GetFragments();

            var exportList = fragments
                .Select(f => new {
                    f.Hash,
                    f.Name,
                    f.Description,
                    EnergyReturnFocus   = string.Empty,
                    TriggerType         = string.Empty,
                    UniversalVersatile  = (bool?)null,
                    Element             = string.Empty
                })
                .OrderBy(x => x.Name)
                .ToList();

            var opts = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(exportList, opts);
            File.WriteAllText(outputPath, json);
        }
    }
}
