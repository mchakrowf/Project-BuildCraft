using System;

namespace ProjectBuildCraft.Models
{
    public class Aspect
    {
        public int Id               { get; set; }

        // ← NEW: link back to the manifest definition
        public long DefHash         { get; set; }

        public string Name          { get; set; } = string.Empty;
        public FocusType Focus      { get; set; }
        public int FragmentSlots    { get; set; }

        // ← NEW: flag prismatic variants
        public bool IsPrismatic     { get; set; }

        // existing subclass FK/API mapping
        public int SubclassDefinitionId     { get; set; }
        public Subclass SubclassDefinition  { get; set; } = default!;

        public string IconPath      { get; set; } = string.Empty;
        public string Description   { get; set; } = string.Empty;
    }
}
