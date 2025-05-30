// Models/InventoryItemDefinition.cs
using System.Text.Json.Serialization;

namespace ProjectBuildCraft.Models
{
    public class InventoryItemDefinition
    {
        [JsonPropertyName("hash")]
        public long Hash { get; set; }

        [JsonPropertyName("displayProperties")]
        public DisplayProperties Display { get; set; } = new();

        [JsonPropertyName("plug")]
        public PlugInfo Plug { get; set; } = new();

        [JsonPropertyName("defaultDamageType")]
        public int DefaultDamageType { get; set; }


        public class DisplayProperties
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("description")]
            public string Description { get; set; } = string.Empty;

            [JsonPropertyName("icon")]
            public string Icon { get; set; } = string.Empty;
        }

        public class PlugInfo
        {
            [JsonPropertyName("plugCategoryIdentifier")]
            public string PlugCategoryIdentifier { get; set; } = string.Empty;
        }


    }
}
