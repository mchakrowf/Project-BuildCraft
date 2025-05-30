// In Services/AspectMetadataService.cs
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ProjectBuildCraft.Models;

namespace ProjectBuildCraft.Services
{
    public interface IAspectMetadataService
    {
        IReadOnlyList<AspectMetadata> All { get; }
    }

    public class AspectMetadataService : IAspectMetadataService
    {
        public IReadOnlyList<AspectMetadata> All { get; }

        public AspectMetadataService(IConfiguration config)
        {
            var path = config["AspectMetadataPath"]
                    ?? throw new InvalidOperationException("Missing AspectMetadataPath in configuration");
            var json = File.ReadAllText(path);
            All = JsonSerializer.Deserialize<List<AspectMetadata>>(json)
                  ?? new List<AspectMetadata>();
        }
    }
}
