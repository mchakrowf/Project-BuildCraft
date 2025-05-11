using Microsoft.EntityFrameworkCore;
using ProjectBuildCraft.Models;

namespace ProjectBuildCraft.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<DestinyClass> Classes { get; set; }
        public DbSet<Subclass> Subclasses { get; set; }
        public DbSet<ExoticArmor> Armors { get; set; }
        public DbSet<ExoticWeapon> Weapons { get; set; }
        public DbSet<StatSelection> Stats { get; set; }
        public DbSet<FocusOption> FocusOptions { get; set; }
        public DbSet<BuildMapping> Mappings { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Destiny Classes: Warlock=1, Hunter=2, Titan=3
            modelBuilder.Entity<DestinyClass>().HasData(
                new DestinyClass { Id = 1, Name = "Warlock" },
                new DestinyClass { Id = 2, Name = "Hunter" },
                new DestinyClass { Id = 3, Name = "Titan" }
            );

            // Subclasses seeded by class order
            modelBuilder.Entity<Subclass>().HasData(
                new Subclass { Id = 1, DestinyClassId = 1, Name = "Dawnblade" },
                new Subclass { Id = 2, DestinyClassId = 1, Name = "Voidwalker" },
                new Subclass { Id = 3, DestinyClassId = 1, Name = "Stormcaller" },
                new Subclass { Id = 4, DestinyClassId = 1, Name = "Shadebinder" },
                new Subclass { Id = 5, DestinyClassId = 1, Name = "Broodweaver" },
                new Subclass { Id = 6, DestinyClassId = 1, Name = "Prismatic Warlock" },
                
                new Subclass { Id = 7, DestinyClassId = 2, Name = "Gunslinger" },
                new Subclass { Id = 8, DestinyClassId = 2, Name = "Nightstalker" },
                new Subclass { Id = 9, DestinyClassId = 2, Name = "Arcstrider" },
                new Subclass { Id = 10, DestinyClassId = 2, Name = "Revenant" },
                new Subclass { Id = 11, DestinyClassId = 2, Name = "Threadrunner" },
                new Subclass { Id = 12, DestinyClassId = 2, Name = "Prismatic Hunter" },

                new Subclass { Id = 13, DestinyClassId = 3, Name = "Sentinel" },
                new Subclass { Id = 14, DestinyClassId = 3, Name = "Sunbreaker" },
                new Subclass { Id = 15, DestinyClassId = 3, Name = "Striker" },
                new Subclass { Id = 16, DestinyClassId = 3, Name = "Behemoth" },
                new Subclass { Id = 17, DestinyClassId = 3, Name = "Berserker" },
                new Subclass { Id = 18, DestinyClassId = 3, Name = "Prismatic Titan" }
            );

            // Exotic Armor: seeded by class
            modelBuilder.Entity<ExoticArmor>().HasData(
                new ExoticArmor { Id = 1, DestinyClassId = 1, Name = "Sunbracers" },
                new ExoticArmor { Id = 2, DestinyClassId = 1, Name = "Transversive Steps" },
                new ExoticArmor { Id = 3, DestinyClassId = 2, Name = "Celestial Nighthawk" },
                new ExoticArmor { Id = 4, DestinyClassId = 2, Name = "Raiden Flux" },
                new ExoticArmor { Id = 5, DestinyClassId = 3, Name = "Heart of Inmost Light" },
                new ExoticArmor { Id = 6, DestinyClassId = 3, Name = "Dunemarchers" }
            );

            // Exotic Weapons
            modelBuilder.Entity<ExoticWeapon>().HasData(
                new ExoticWeapon { Id = 1, Name = "Dragon's Breath" },
                new ExoticWeapon { Id = 2, Name = "Ace of Spades" },
                new ExoticWeapon { Id = 3, Name = "Thorn" }
            );

            // Stat Selections
            modelBuilder.Entity<StatSelection>().HasData(
                new StatSelection { Id = 1, Name = "Mobility" },
                new StatSelection { Id = 2, Name = "Resilience" },
                new StatSelection { Id = 3, Name = "Recovery" },
                new StatSelection { Id = 4, Name = "Discipline" },
                new StatSelection { Id = 5, Name = "Intellect" },
                new StatSelection { Id = 6, Name = "Strength" }
            );

            // Focus Options
            modelBuilder.Entity<FocusOption>().HasData(
                new FocusOption { Id = 1, Name = "Weapon Damage" },
                new FocusOption { Id = 2, Name = "Weapon Uptime" },
                new FocusOption { Id = 3, Name = "Ability Uptime" }
            );

            // NEW: BuildMapping relationships for Primary and Secondary Stats
            modelBuilder.Entity<BuildMapping>()
                .HasOne(m => m.PrimaryStat)
                .WithMany()
                .HasForeignKey(m => m.PrimaryStatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BuildMapping>()
                .HasOne(m => m.SecondaryStat)
                .WithMany()
                .HasForeignKey(m => m.SecondaryStatId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BuildMapping>().HasData(
                new BuildMapping {
                    Id               = 2,
                    ClassId          = 1, // Warlock
                    SubclassId       = 1, // Dawnblade
                    ArmorId          = 1, // Sunbracers
                    WeaponId         = 1, // Dragon's Breath
                    FocusOptionId    = 3, // Ability Uptime
                    PrimaryStatId    = 3, // Recovery
                    SecondaryStatId  = 6, // Strength

                    // store your “strings” here:
                    Mods = string.Join("; ", new[] {
                        "Aspects: Touch of Flame",
                        "Fragments: Cure, Mercy, Flame, Searing",
                        "Helmet: Ashes to Assets ×2, Siphon Mod",
                        "Gloves: Firepower ×2, Momentum Transfer",
                        "Chest: Concussive Dampener, Emergency Reinforcement",
                        "Legs: Absolution, Restoration, Invigoration",
                        "Class Item: Powerful Attraction, Outreach, Distribution"
                    }),

                    // optional human‐readable summary
                    Summary = "Dawnblade build centered on Ability Uptime with Touch of Flame + high-regen fragments and Dragon's Breath."
                }
            );

        }
    }
}


