using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBuildCraft.Models
{
    public class BuildMapping
    {
        [Key]
        public int Id { get; set; }

        // your selection keys
        public int ClassId        { get; set; }
        public int SubclassId     { get; set; }
        public int ArmorId        { get; set; }
        public int WeaponId       { get; set; }
        public int FocusOptionId  { get; set; }
        public int ChargeTypeId   { get; set; }

        // result data
        [Required]
        public string Mods    { get; set; } = null!;

        public string? Summary { get; set; }

        // navigation props
        [ForeignKey(nameof(ClassId))]
        public DestinyClass Class { get; set; } = null!;

        [ForeignKey(nameof(SubclassId))]
        public Subclass Subclass { get; set; } = null!;

        [ForeignKey(nameof(ArmorId))]
        public ExoticArmor Armor { get; set; } = null!;

        [ForeignKey(nameof(WeaponId))]
        public ExoticWeapon Weapon { get; set; } = null!;

        [ForeignKey(nameof(FocusOptionId))]
        public FocusOption FocusOption { get; set; } = null!;

        [ForeignKey(nameof(ChargeTypeId))]
        public ChargeType ChargeType { get; set; } = null!;

        public virtual ICollection<Aspect>   Aspects   { get; set; } = new List<Aspect>();
        public virtual ICollection<Fragment> Fragments { get; set; } = new List<Fragment>();
    }
}
