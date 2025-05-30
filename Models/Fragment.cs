// Models/Fragment.cs
namespace ProjectBuildCraft.Models
{
    public class Fragment
    {
        public long Hash { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconPath { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
