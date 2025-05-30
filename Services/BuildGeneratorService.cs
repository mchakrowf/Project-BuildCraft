// Services/BuildGeneratorService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using ProjectBuildCraft.Data;
using ProjectBuildCraft.Models;
using ProjectBuildCraft.Services;    // for IExoticMetadataService, IAspectMetadataService

namespace ProjectBuildCraft.Services
{
    public class BuildGeneratorService : IBuildGeneratorService
    {
        private readonly AppDbContext _db;
        private readonly IManifestService _manifest;
        private readonly IAspectMetadataService _aspectMeta;
        private readonly IExoticMetadataService _exoticMeta;

        private readonly Dictionary<long, FragmentMetadata> _fragmentMetadata;
        private readonly Dictionary<long, PerkDef> _fragmentDefs;

        public BuildGeneratorService(
            AppDbContext db,
            IManifestService manifest,
            IAspectMetadataService aspectMeta,
            IExoticMetadataService exoticMeta
        )
        {
            _db         = db;
            _manifest   = manifest;
            _aspectMeta = aspectMeta;
            _exoticMeta = exoticMeta;

            // load fragment metadata...
            var fragPath = Path.Combine(AppContext.BaseDirectory, "Data", "fragments-metadata.json");
            var fragJson = File.ReadAllText(fragPath);
            var fragList = JsonSerializer.Deserialize<List<FragmentMetadata>>(fragJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                          ?? new List<FragmentMetadata>();
            _fragmentMetadata = fragList.ToDictionary(f => f.Hash);

            _fragmentDefs = _manifest.GetFragments().ToDictionary(f => f.Hash, f => f);
        }

        public Dictionary<string, List<ModDef>> GenerateModLoadout(
            int subclassId,
            int focusOptionId,
            string exoticElement,
            int chargeTypeId
        )
        {
            // 1) seeded mapping?
            var map = _db.Mappings.FirstOrDefault(m =>
                m.SubclassId    == subclassId &&
                m.FocusOptionId == focusOptionId
            );
            if (map != null && !string.IsNullOrWhiteSpace(map.Mods))
            {
                var dict = new Dictionary<string, List<ModDef>>();
                foreach (var part in map.Mods.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    var pieces   = part.Split(':', 2, StringSplitOptions.TrimEntries);
                    var slotName = pieces[0];
                    var mods = pieces.Length > 1
                        ? pieces[1]
                            .Split(',', StringSplitOptions.TrimEntries)
                            .Select(name => _manifest.GetModByName(name!))
                            .Where(md => md != null)
                            .ToList()!
                        : new List<ModDef>();
                    dict[slotName] = mods;
                }
                return dict;
            }

            // 2) dynamic fallback
            List<ModDef> lookup(params string?[] names) =>
                names
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Select(n => _manifest.GetModByName(n!))
                    .Where(md => md != null)
                    .ToList()!;

            var result = new Dictionary<string, List<ModDef>>();
            switch (focusOptionId)
            {
                case 1: // Weapon Damage
                    result["Helmet"]    = lookup("Heavy Ammo Finder", "Heavy Ammo Scout", $"{exoticElement} Siphon");
                    result["Arms"]      = lookup("Shield Break Charge", $"{exoticElement} Loader");
                    result["Chest"]     = lookup("Charged Up");
                    result["Legs"]      = lookup($"{exoticElement} Weapon Surge", "Recuperation");
                    result["ClassItem"] = lookup("Time Dilation", "Reaper");
                    break;

                case 2: // Weapon Uptime
                    result["Helmet"]    = lookup("Heavy Ammo Finder", "Heavy Ammo Scout", $"{exoticElement} Siphon");
                    result["Arms"]      = lookup($"{exoticElement} Loader");
                    result["Chest"]     = lookup("Charged Up", $"{exoticElement} Reserves");
                    result["Legs"]      = lookup($"{exoticElement} Scavenger", $"{exoticElement} Holster", "Recuperation");
                    result["ClassItem"] = lookup("Special Finisher", "Reaper");
                    break;

                case 3: // Ability Uptime
                    var spark = lookup(
                        $"{exoticElement} Siphon",
                        chargeTypeId switch {
                            1 => "Ashes to Assets",
                            2 => "Hands-On",
                            _ => "Dynamo"
                        }
                    );
                    result["Helmet"]    = spark;
                    result["Arms"]      = lookup(
                        chargeTypeId == 1 ? "Grenade Kickstart"
                                         : chargeTypeId == 2 ? "Melee Kickstart"
                                                             : "Bolstering Detonation",
                        chargeTypeId == 1 ? "Firepower"
                                         : chargeTypeId == 2 ? "Heavy Handed"
                                                             : "Focusing Strike",
                        chargeTypeId == 1 ? "Impact Induction" : null
                    );
                    result["Chest"]     = lookup("Charged Up");
                    result["Legs"]      = lookup(
                        "Absolution",
                        "Orbs of Restoration",
                        chargeTypeId == 1 ? "Innervation"
                                         : chargeTypeId == 2 ? "Invigoration"
                                                             : "Insulation"
                    );
                    result["ClassItem"] = lookup(
                        "Distribution",
                        "Reaper",
                        chargeTypeId == 1 ? "Bomber"
                                         : chargeTypeId == 2 ? "Outreach"
                                                             : "Utility Kickstart"
                    );
                    break;

                default: // Balanced fallback
                    result["Helmet"]    = lookup("Ashes to Assets", "Dynamo", $"{exoticElement} Siphon");
                    result["Arms"]      = lookup("Grenade Kickstart", "Impact Induction", "Momentum Transfer");
                    result["Chest"]     = lookup("Charged Up");
                    result["Legs"]      = lookup("Absolution", "Innervation", "Invigoration");
                    result["ClassItem"] = lookup("Reaper", "Bomber", "Outreach");
                    break;
            }

            return result;
        }

        public BuildMapping GenerateBuild(
            GuardianClass @class,
            int subclassId,
            int exoticArmorHash,
            int exoticWeaponHash,
            int focusOptionId,
            int chargeTypeId)
        {
            // 1) Pull the selected exotic armor + weapon
            var armor = _manifest
                .GetExoticArmor(@class)
                .FirstOrDefault(a => a.Hash == exoticArmorHash);
            var weapon = _manifest
                .GetExoticWeapons()
                .FirstOrDefault(w => w.Hash == exoticWeaponHash);

            // 2) Look up the weapon’s element via metadata
            var exoticMeta    = _exoticMeta.All.Single(e => e.Hash == exoticWeaponHash);
            var exoticElement = exoticMeta.Element;

            // 3) Build the mod loadout
            var mods = GenerateModLoadout(
                subclassId,
                focusOptionId,
                exoticElement,
                chargeTypeId
            );

            // 4) Pick exactly two aspects
            var allAspects    = _manifest.GetAspects();
            var chosenAspects = SelectAspects(
                @class,
                subclassId,
                chargeTypeId,
                exoticArmorHash,
                allAspects
            );

            // 5) Determine subclass’s native element
            var subclassName = _db.Subclasses.Find(subclassId)!.Name;
            string subclassElement = subclassName switch
            {
                /*----------Warlock----------*/
                "Dawnblade"        => "Solar",
                "Voidwalker"       => "Void",
                "Stormcaller"      => "Arc",
                "Shadebinder"      => "Stasis",
                "Broodweaver"      => "Strand",
                "Prismatic Warlock"=> exoticElement,

                /*-----------Hunter-----------*/
                "Gunslinger"       => "Solar",
                "Nightstalker"     => "Void",
                "Arcstrider"       => "Arc",
                "Revenant"         => "Stasis",
                "Threadrunner"     => "Strand",
                "Prismatic Hunter" => exoticElement,

                /*-----------Titan-----------*/
                "Sunbreaker"       => "Solar",
                "Sentinel"         => "Void",
                "Striker"          => "Arc",
                "Behemoth"         => "Stasis",
                "Berserker"        => "Strand",
                "Prismatic Titan"  => exoticElement,

                _                  => exoticElement
            };

            // 6) Sum total fragment slots
            var totalSlots = chosenAspects.Sum(a => a.FragmentSlots);

            // 7) Pick fragments
            var fragmentMetas = SelectFragments(focusOptionId, chargeTypeId, subclassId, totalSlots);
            var chosenFrags = fragmentMetas
                .Select(meta => new Fragment
                {
                    Hash        = meta.Hash,
                    Name        = meta.Name,
                    IconPath    = _fragmentDefs[meta.Hash].IconPath,
                    Description = _fragmentDefs[meta.Hash].Description
                })
                .ToList();

            // 8) Assemble and return
            return new BuildMapping
            {
                ClassId       = (int)@class,
                SubclassId    = subclassId,
                ArmorId       = exoticArmorHash,
                WeaponId      = exoticWeaponHash,
                FocusOptionId = focusOptionId,
                ChargeTypeId  = chargeTypeId,
                Mods          = string.Join(";", 
                                mods.SelectMany(kv => kv.Value
                                                        .Select(m => $"{kv.Key}:{m.Name}")
                                )),
                Aspects       = chosenAspects,
                Fragments     = chosenFrags,
                Summary       = $"{@class}: {string.Join(" + ", chosenAspects.Select(a => a.Name))}"
            };
        }


        private List<FragmentMetadata> SelectFragments(
            int focusOptionId,
            int chargeTypeId,    // 1=Grenade, 2=Melee, 3=ClassAbility, 4=Super
            int subclassId,
            int slotCount)
        {
            // DEBUG: entry parameters
            Console.WriteLine($"[DEBUG] SelectFragments(focusOptionId={focusOptionId}, chargeTypeId={chargeTypeId}, subclassId={subclassId}, slotCount={slotCount})");

            // 1) Determine subclass element (Prismatic subclasses → "Prismatic")
            var subclass = _db.Subclasses.Find(subclassId)
                        ?? throw new InvalidOperationException($"Subclass {subclassId} not found");
            bool isPrismatic = subclass.Name.StartsWith("Prismatic", StringComparison.OrdinalIgnoreCase);
            var fragmentElement = isPrismatic
                ? "Prismatic"
                : subclass.Name switch
            {
                /*-------Warlock-------*/
                "Dawnblade"   => "Solar",
                "Voidwalker"  => "Void",
                "Stormcaller" => "Arc",
                "Shadebinder" => "Stasis",
                "Broodweaver" => "Strand",

                /*--------Hunter--------*/
                "Gunslinger"  => "Solar",
                "Nightstalker"=> "Void",
                "Arcstrider"  => "Arc",
                "Revenant"    => "Stasis",
                "Threadrunner"=> "Strand",

                /*---------Titan---------*/
                "Sunbreaker"  => "Solar",
                "Sentinel"    => "Void",
                "Striker"     => "Arc",
                "Behemoth"    => "Stasis",
                "Berserker"   => "Strand",

                _             => throw new InvalidOperationException($"Unknown subclass {subclass.Name}")
            };
            Console.WriteLine($"[DEBUG] fragmentElement = \"{fragmentElement}\" (isPrismatic={isPrismatic})");

            // 2) Map chargeTypeId → EnergyReturnFocus strings
            var energyFocusName = chargeTypeId switch
            {
                1 => "Grenade",
                2 => "Melee",
                3 => "ClassAbility",
                4 => "Super",
                _ => throw new InvalidOperationException($"Unexpected chargeTypeId {chargeTypeId}; must be 1–4")
            };
            Console.WriteLine($"[DEBUG] chargeTypeId = {chargeTypeId} → energyFocusName = \"{energyFocusName}\"");

            // 3) Gather all fragments matching the subclass element
            var all = _fragmentMetadata.Values
                .Where(f => string.Equals(f.Element, fragmentElement, StringComparison.OrdinalIgnoreCase))
                .ToList();
            Console.WriteLine($"[DEBUG] All {all.Count} fragments for element \"{fragmentElement}\": {string.Join(", ", all.Select(f => f.Name))}");

            var result = new List<FragmentMetadata>();

            // Step 1) primary focus
            var step1 = all
                .Where(f => string.Equals(f.EnergyReturnFocus, energyFocusName, StringComparison.OrdinalIgnoreCase))
                .Take(slotCount)
                .ToList();
            Console.WriteLine($"[DEBUG] Step 1 (EnergyReturnFocus==\"{energyFocusName}\") → {step1.Count}: {string.Join(", ", step1.Select(f => f.Name))}");
            result.AddRange(step1);

            // Step 2) universal versatile
            if (result.Count < slotCount)
            {
                var need = slotCount - result.Count;
                var step2 = all
                    .Where(f => f.UniversalVersatile == true && !result.Contains(f))
                    .Take(need)
                    .ToList();
                Console.WriteLine($"[DEBUG] Step 2 (UniversalVersatile) → {step2.Count}: {string.Join(", ", step2.Select(f => f.Name))}");
                result.AddRange(step2);
            }

            // Step 3) neutral fallback
            if (result.Count < slotCount)
            {
                var need = slotCount - result.Count;
                var step3 = all
                    .Where(f => string.Equals(f.EnergyReturnFocus, "Neutral", StringComparison.OrdinalIgnoreCase)
                            && !result.Contains(f))
                    .Take(need)
                    .ToList();
                Console.WriteLine($"[DEBUG] Step 3 (Neutral) → {step3.Count}: {string.Join(", ", step3.Select(f => f.Name))}");
                result.AddRange(step3);
            }

            // Step 4) fill any remaining
            if (result.Count < slotCount)
            {
                var need = slotCount - result.Count;
                var step4 = all
                    .Where(f => !result.Contains(f))
                    .Take(need)
                    .ToList();
                Console.WriteLine($"[DEBUG] Step 4 (FillRemaining) → {step4.Count}: {string.Join(", ", step4.Select(f => f.Name))}");
                result.AddRange(step4);
            }

            Console.WriteLine($"[DEBUG] Final fragments chosen ({result.Count}): {string.Join(", ", result.Select(f => f.Name))}");
            return result;
        }

        private List<Aspect> SelectAspects(
            GuardianClass guardianClass,
            int subclassId,
            int abilityChargeOptionId,    // 1=Grenade, 2=Melee, 3=ClassAbility, 4=Super
            int exoticArmorHash,
            IReadOnlyList<PerkDef> allAspects)
        {
            // 1) Determine class & detect Prismatic subclasses
            var classTypeName = guardianClass.ToString();
            var subclass      = _db.Subclasses.Find(subclassId)
                            ?? throw new InvalidOperationException($"Subclass {subclassId} not found");
            bool isPrismaticSubclass = subclass.Name
                .StartsWith("Prismatic", StringComparison.OrdinalIgnoreCase);

            // 2) Map non-prismatic to element (ignore element for prismatic)
            string subclassElement = isPrismaticSubclass
                ? null!
                : subclass.Name switch
            {
                "Dawnblade"   => "Solar",
                "Voidwalker"  => "Void",
                "Stormcaller" => "Arc",
                "Shadebinder" => "Stasis",
                "Broodweaver" => "Strand",

                "Gunslinger"  => "Solar",
                "Nightstalker"=> "Void",
                "Arcstrider"  => "Arc",
                "Revenant"    => "Stasis",
                "Threadrunner"=> "Strand",

                "Sunbreaker"  => "Solar",
                "Sentinel"    => "Void",
                "Striker"     => "Arc",
                "Behemoth"    => "Stasis",
                "Berserker"   => "Strand",
                _             => throw new InvalidOperationException($"Unknown subclass {subclass.Name}")
            };
            Console.WriteLine($"[DEBUG] isPrismaticSubclass={isPrismaticSubclass}, subclassElement=\"{subclassElement}\"");

            // 3) Map the chargeTypeId → exactly the PrimaryFocus strings
            var chargeName = abilityChargeOptionId switch
            {
                1 => "Grenade",
                2 => "Melee",
                3 => "ClassAbility",
                4 => "Super",
                _ => throw new InvalidOperationException($"Unexpected chargeTypeId {abilityChargeOptionId}; must be 1–4")
            };
            Console.WriteLine($"[DEBUG] chargeTypeId={abilityChargeOptionId} → chargeName=\"{chargeName}\"");

            // 4) Candidate pool: match class, prismatic vs non-prismatic, and element if non
            var allCandidates = _aspectMeta.All
                .Where(m =>
                    m.ClassType.Equals(classTypeName, StringComparison.OrdinalIgnoreCase)
                && (isPrismaticSubclass
                    ? m.IsPrismatic == true
                    : m.IsPrismatic != true
                        && m.Element.Equals(subclassElement, StringComparison.OrdinalIgnoreCase)
                    )
                )
                .DistinctBy(m => m.Hash)
                .ToList();

            Console.WriteLine($"[DEBUG] All {allCandidates.Count} candidates for {classTypeName}/{(isPrismaticSubclass?"Prismatic":subclassElement)}:");
            foreach (var m in allCandidates)
            {
                Console.WriteLine(
                    $" • {m.Name} (Hash={m.Hash}) " +
                    $"PrimaryFocus={m.PrimaryFocus}, Fallback={m.FallbackFocus}, IsPrismatic={m.IsPrismatic}" +
                    (m.RequiredByExotics?.Any() == true
                    ? $", RequiredBy=[{string.Join(",",m.RequiredByExotics)}]" : "")
                );
            }

            var chosen = new List<AspectMetadata>();

            // Step 1) RequiredByExotics
            var step1 = allCandidates
                .Where(m => m.RequiredByExotics?.Contains(exoticArmorHash) == true)
                .ToList();
            Console.WriteLine($"[DEBUG] Step 1 (RequiredByExotics) → {step1.Count}: {string.Join(", ", step1.Select(m => m.Name))}");
            chosen.AddRange(step1.Take(2 - chosen.Count));

            // Step 2) PrimaryFocus == chargeName
            var step2 = allCandidates
                .Where(m => m.PrimaryFocus.Equals(chargeName, StringComparison.OrdinalIgnoreCase)
                        && !chosen.Any(c => c.Hash == m.Hash))
                .ToList();
            Console.WriteLine($"[DEBUG] Step 2 (PrimaryFocus==\"{chargeName}\") → {step2.Count}: {string.Join(", ", step2.Select(m => m.Name))}");
            chosen.AddRange(step2.Take(2 - chosen.Count));

            // Step 3) FallbackFocus contains chargeName
            var step3 = allCandidates
                .Where(m =>
                    !string.IsNullOrWhiteSpace(m.FallbackFocus)
                && m.FallbackFocus
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Any(ff => ff.Trim().Equals(chargeName, StringComparison.OrdinalIgnoreCase))
                && !chosen.Any(c => c.Hash == m.Hash))
                .ToList();
            Console.WriteLine($"[DEBUG] Step 3 (Fallback) → {step3.Count}: {string.Join(", ", step3.Select(m => m.Name))}");
            chosen.AddRange(step3.Take(2 - chosen.Count));

            // Step 4) PrimaryFocus=="Neutral" OR FallbackFocus=="Neutral"
            var step4 = allCandidates
                .Where(m =>
                    (m.PrimaryFocus.Equals("Neutral", StringComparison.OrdinalIgnoreCase)
                || (!string.IsNullOrWhiteSpace(m.FallbackFocus)
                    && m.FallbackFocus
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Any(ff => ff.Trim().Equals("Neutral", StringComparison.OrdinalIgnoreCase))))
                && !chosen.Any(c => c.Hash == m.Hash))
                .ToList();
            Console.WriteLine($"[DEBUG] Step 4 (Neutral) → {step4.Count}: {string.Join(", ", step4.Select(m => m.Name))}");
            chosen.AddRange(step4.Take(2 - chosen.Count));

            // Step 5) Fill remaining
            var step5 = allCandidates
                .Where(m => !chosen.Any(c => c.Hash == m.Hash))
                .ToList();
            Console.WriteLine($"[DEBUG] Step 5 (Fill remaining) → {step5.Count}: {string.Join(", ", step5.Select(m => m.Name))}");
            chosen.AddRange(step5.Take(2 - chosen.Count));

            Console.WriteLine($"[DEBUG] Final chosen ({chosen.Count}): {string.Join(", ", chosen.Select(m => m.Name))}");

            // 5) Materialize into EF Aspect entities, with a fallback if PerkDef is missing
            var results = new List<Aspect>();
            foreach (var meta in chosen)
            {
                var def = allAspects.FirstOrDefault(a => a.Hash == meta.Hash);
                if (def == null)
                {
                    results.Add(new Aspect {
                        DefHash              = meta.Hash,
                        Name                 = meta.Name,
                        FragmentSlots        = meta.FragmentSlots ?? 0,
                        IsPrismatic          = meta.IsPrismatic  ?? false,
                        Focus                = Enum.TryParse<FocusType>(meta.PrimaryFocus, true, out var f) ? f : FocusType.Neutral,
                        SubclassDefinitionId = subclassId,
                        IconPath             = "",               // no icon available
                        Description          = meta.Description ?? ""
                    });
                }
                else
                {
                    results.Add(new Aspect {
                        DefHash              = def.Hash,
                        Name                 = def.Name,
                        FragmentSlots        = meta.FragmentSlots ?? def.FragmentSlots,
                        IsPrismatic          = meta.IsPrismatic  ?? false,
                        Focus                = Enum.Parse<FocusType>(meta.PrimaryFocus, true),
                        SubclassDefinitionId = subclassId,
                        IconPath             = def.IconPath,
                        Description          = def.Description
                    });
                }
            }

            return results;
        }

    }
}
