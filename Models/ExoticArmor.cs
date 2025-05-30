namespace ProjectBuildCraft.Models
{
    public class ExoticArmor
    {
        // ← add this
        public int Hash            { get; set; }

        // you can remove the old Id if it’s unused,
        // or keep it alongside—up to you.
        public int Id              { get; set; }

        public string Name         { get; set; } = string.Empty;
        public string IconPath     { get; set; } = string.Empty;
        public string Description  { get; set; } = string.Empty;
        public int    DestinyClassId { get; set; }
    }
}
