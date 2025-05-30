namespace ProjectBuildCraft.Models
{
    public class SubclassDefinition
    {
        public int Id { get; set; }

        // Initialize to a non-null default
        public string Name { get; set; } = string.Empty;

        // Slot counts
        public int AspectSlots  { get; set; }
        public int FragmentSlots { get; set; }

        // Navigation properties — initialize your collection
        public ICollection<Aspect> Aspects { get; set; } = new List<Aspect>();

        // … other existing properties …
        public ElementType ElementType { get; set; }
        public ClassType   ClassType   { get; set; }
        public string      IconPath    { get; set; } = string.Empty;
    }
}
