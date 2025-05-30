// ProjectBuildCraft/Data/AspectExporter.cs
using System.IO;
using System.Linq;
using System.Text.Json;
using ProjectBuildCraft.Services;
using ProjectBuildCraft.Models;

namespace ProjectBuildCraft.Data
{
    /// <summary>
    /// Dumps every Aspect’s hash, name, description,
    /// plus empty placeholders for your hand‐curated metadata,
    /// including RequiredByExotics.
    /// </summary>
    public class AspectExporter
    {
        private readonly IManifestService _manifest;
        public AspectExporter(IManifestService manifest)
            => _manifest = manifest;

        public void ExportDefinitions(string outputPath)
        {
            var aspects = _manifest.GetAspects();

            var exportList = aspects
                .Select(a => new {
                    a.Hash,
                    a.Name,
                    a.Description,
                    ClassType         = string.Empty,
                    Element           = string.Empty,
                    PrimaryFocus      = string.Empty,
                    FallbackFocus     = (string?)null,
                    IsPrismatic       = (bool?)null,
                    FragmentSlots     = (int?)null,
                    RequiredByExotics = (long[]?)null
                })
                .OrderBy(x => x.Name)
                .ToList();

            var opts = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(exportList, opts);
            File.WriteAllText(outputPath, json);
        }
    }
}
