namespace ProjectBuildCraft.Models
{
    public class ModDef
    {
        public long   Hash                    { get; set; }
        public string Name                    { get; set; } = string.Empty;
        public string IconPath                { get; set; } = string.Empty;
        // ← Add these two:
        public string Description             { get; set; } = string.Empty;
        public string PlugCategoryIdentifier  { get; set; } = string.Empty;
    }
}
