namespace ProjectBuildCraft.Models
{

    public class FragmentMetadata
    {
        public long    Hash               { get; set; }
        public string Name               { get; set; } = string.Empty;
        public string Description        { get; set; } = string.Empty;
        public string EnergyReturnFocus  { get; set; } = string.Empty;
        public string TriggerType        { get; set; } = string.Empty;
        public bool?  UniversalVersatile { get; set; }
        public string Element            { get; set; } = string.Empty;
    }
}
