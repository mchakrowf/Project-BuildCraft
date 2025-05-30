// Models/DestinyManifest.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProjectBuildCraft.Models
{
    public class DestinyManifest
    {
        // Maps language (“en”) → definition name → JSON path
        [JsonPropertyName("jsonWorldComponentContentPaths")]
        public Dictionary<string, Dictionary<string, string>> JsonWorldComponentContentPaths { get; set; }
            = new Dictionary<string, Dictionary<string, string>>();
    }
}
