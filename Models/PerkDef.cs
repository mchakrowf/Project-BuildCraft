// Models/PerkDef.cs
namespace ProjectBuildCraft.Models
{
    public class PerkDef
    {
        public long   Hash     { get; set; }
        public string Name     { get; set; } = "";
        public string IconPath { get; set; } = "";
        public string Description { get; set; } = "";   // ← new

        public int    FragmentSlots { get; set; }

    }
}
