namespace ProjectBuildCraft.Models
{
    public class EntityArchetype
    {
        public long EntityId { get; set; } 
        public string EntityType { get; set; } = null!;    // "Exotic" | "Subclass" | …
        public int ArchetypeId { get; set; }
        public Archetype Archetype { get; set; } = null!;
    }
}