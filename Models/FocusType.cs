using System;

namespace ProjectBuildCraft.Models
{
    [Flags]
    public enum FocusType
    {
        Neutral       = 0,
        Super         = 1 << 0,
        Grenade       = 1 << 1,
        Melee         = 1 << 2,
        ClassAbility  = 1 << 3
    }
}
