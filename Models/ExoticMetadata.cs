namespace ProjectBuildCraft.Models
{
    public class ExoticMetadata
    {
        public long   Hash { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Element   { get; set; } = string.Empty;  // ← new
    }

    public class ExoticMetadataRoot
    {
        public List<ExoticMetadata> Armor   { get; set; } = new();
        public List<ExoticMetadata> Weapons { get; set; } = new();
    }
}
