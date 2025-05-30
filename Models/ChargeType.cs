namespace ProjectBuildCraft.Models
{
    public class ChargeType
    {
        public int Id   { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<ChargeModTemplate> ModTemplates { get; set; }
            = new List<ChargeModTemplate>();
    }
}
