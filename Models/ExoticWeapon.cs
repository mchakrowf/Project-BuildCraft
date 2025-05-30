namespace ProjectBuildCraft.Models
{
    public class ExoticWeapon
    {
        public int Id { get; set; }
        public int Hash { get; set; }
        public ElementType Element { get; set; }
        public string Name { get; set; } = null!;
        
    }
}