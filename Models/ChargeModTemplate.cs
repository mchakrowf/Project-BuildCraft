namespace ProjectBuildCraft.Models
{
    public class ChargeModTemplate
    {
        public int ChargeTypeId { get; set; }
        public string Slot      { get; set; } = string.Empty;
        public string ModName   { get; set; } = string.Empty;

        public ChargeType ChargeType { get; set; } = null!;
    }
}
