namespace ProjectBuildCraft.Models
{
    public class Archetype
    {
        public int Id { get; set; }

        // initialize string to empty
        public string Name { get; set; } = string.Empty;

        // initialize collections
        public List<EntityArchetype> EntityArchetypes { get; set; } = new List<EntityArchetype>();
        public List<ArchetypeModTemplate> ModTemplates     { get; set; } = new List<ArchetypeModTemplate>();
    }
}
