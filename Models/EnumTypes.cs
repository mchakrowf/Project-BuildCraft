// Models/EnumTypes.cs
namespace ProjectBuildCraft.Models
{
    public enum ClassType
    {
        Hunter,
        Titan,
        Warlock
    }

    public enum ElementType
    {
        Solar,
        Arc,
        Void,
        Stasis,
        Strand,
        Prismatic,
        Kinetic
    }

    public enum AbilityFocus
    {
        Grenade,
        Melee,
        ClassAbility,
        Super,
        Neutral
    }

    public enum TriggerType
    {
        AbilityHit,
        AbilityKill,
        OrbPickup,
        Finisher,
        BuffApply
    }

}
