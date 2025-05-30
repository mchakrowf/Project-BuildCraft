namespace ProjectBuildCraft.Models
{
    public class ArchetypeModTemplate
    {
        public int ArchetypeId { get; set; }

        // defaults so they’re never null
        public string Slot    { get; set; } = string.Empty;
        public string ModName { get; set; } = string.Empty;

        // nav‐prop: mark with null-forgiving or give a default stub
        public Archetype Archetype { get; set; } = null!;
    }
}
