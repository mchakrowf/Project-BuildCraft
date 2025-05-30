// Models/ExoticItem.cs
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectBuildCraft.Models
{
    public enum GuardianClass
    {
        Titan   = 0,  // JSON classType==1
        Hunter  = 1,  // JSON classType==2
        Warlock = 2   // JSON classType==3
    }

    public class ExoticItem
    {
        [JsonPropertyName("hash")]
        public long Hash { get; set; }

        [JsonPropertyName("displayProperties")]
        public DisplayProperties Display { get; set; } = new();

        [JsonPropertyName("classType")]
        public int ClassTypeRaw { get; set; }

        [JsonPropertyName("inventory")]
        public InventoryBlock Inventory { get; set; } = new();

        [JsonPropertyName("itemType")]
        public int ItemTypeRaw { get; set; }

        [JsonPropertyName("defaultDamageType")]
        public int DefaultDamageType { get; set; }

        [JsonIgnore]
        public GuardianClass ClassType => (GuardianClass)ClassTypeRaw;

        [JsonIgnore]
        public bool IsArmor =>
            ItemTypeRaw == 2 && Inventory.TierTypeName == "Exotic";

        [JsonIgnore]
        public bool IsWeapon =>
            ItemTypeRaw == 3 && Inventory.TierTypeName == "Exotic";

        [JsonIgnore]
        public string Name     => Display.Name;

        [JsonIgnore]
        public string IconPath => Display.Icon;
    }

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

    public class InventoryBlock
    {
        [JsonPropertyName("tierTypeName")]
        public string TierTypeName { get; set; } = string.Empty;
    }
}
