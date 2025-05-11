using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBuildCraft.Models
{
    public class BuildMapping
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int SubclassId { get; set; }
        public int ArmorId { get; set; }
        public int WeaponId { get; set; }
        public int FocusOptionId { get; set; }
        public int PrimaryStatId { get; set; }
        public int SecondaryStatId { get; set; }

        public string Mods    { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;

        [ForeignKey(nameof(ClassId))]
        public DestinyClass?   Class       { get; set; }

        [ForeignKey(nameof(SubclassId))]
        public Subclass?       Subclass    { get; set; }

        [ForeignKey(nameof(ArmorId))]
        public ExoticArmor?    Armor       { get; set; }

        [ForeignKey(nameof(WeaponId))]
        public ExoticWeapon?   Weapon      { get; set; }

        [ForeignKey(nameof(FocusOptionId))]
        public FocusOption?    FocusOption { get; set; }

        [ForeignKey(nameof(PrimaryStatId))]
        public StatSelection?  PrimaryStat { get; set; }

        [ForeignKey(nameof(SecondaryStatId))]
        public StatSelection?  SecondaryStat { get; set; }
    }
}
