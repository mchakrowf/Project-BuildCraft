namespace ProjectBuildCraft.Models
{
    public class AspectMetadata
    {
        public long   Hash               { get; set; }
        public string Name               { get; set; } = string.Empty;
        public string Description        { get; set; } = string.Empty;

        // your existing fields…
        public string ClassType          { get; set; } = string.Empty;
        public string PrimaryFocus       { get; set; } = string.Empty;
        public string? FallbackFocus     { get; set; }
        public string Element            { get; set; } = string.Empty;
        public bool?  IsPrismatic       { get; set; }
        public int?   FragmentSlots      { get; set; }

        // ← NEW: which exotic armor hashes *require* this aspect
        public List<long>? RequiredByExotics { get; set; }
    }
}
