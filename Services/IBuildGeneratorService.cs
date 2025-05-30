// Services/IBuildGeneratorService.cs
using System.Collections.Generic;
using ProjectBuildCraft.Models;

namespace ProjectBuildCraft.Services
{
    public interface IBuildGeneratorService
    {
        /// <summary>
        /// Build per‐slot mod list for the given subclass + focus + exotic element.
        /// </summary>
        Dictionary<string, List<ModDef>> GenerateModLoadout(
            int subclassId,
            int focusOptionId,
            string exoticElement,
            int chargeTypeId    // <-- renamed from chargeTypeTd
        );

        /// <summary>
        /// Generate a complete BuildMapping (armor, weapon, mods, aspects, fragments).
        /// </summary>
        BuildMapping GenerateBuild(
            GuardianClass @class,
            int subclassId,
            int exoticArmorHash,
            int exoticWeaponHash,
            int focusOptionId,
            int chargeTypeId
        );
    }
}
