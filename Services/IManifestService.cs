// Services/IManifestService.cs
using ProjectBuildCraft.Models;
using System.Collections.Generic;

namespace ProjectBuildCraft.Services
{
    public interface IManifestService
    {
        IReadOnlyList<PerkDef> GetAspects();
        IReadOnlyList<PerkDef> GetFragments();

        IEnumerable<ExoticArmor> GetExoticArmor(GuardianClass @class);
        IEnumerable<ExoticWeapon> GetExoticWeapons();
        IEnumerable<ModDef>     GetChargeMods(int chargeTypeId);
        ModDef?                 GetModByName(string modName);
    }
}
