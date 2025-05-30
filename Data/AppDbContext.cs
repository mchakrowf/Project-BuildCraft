// Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using ProjectBuildCraft.Models;

namespace ProjectBuildCraft.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<DestinyClass>   Classes       { get; set; }
        public DbSet<Subclass>       Subclasses    { get; set; }
        public DbSet<FocusOption>    FocusOptions  { get; set; }
        public DbSet<ChargeType>     ChargeTypes   { get; set; }

        // These two back your in‐memory loadouts
        public DbSet<Aspect>         Aspects       { get; set; } = null!;
        public DbSet<Fragment>       Fragments     { get; set; } = null!;

        // Your saved builds
        public DbSet<BuildMapping>   Mappings      { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ─── IGNORE any old types you still have in Models folder ─────────────────
            modelBuilder.Ignore<EntityArchetype>();
            modelBuilder.Ignore<ArchetypeModTemplate>();
            modelBuilder.Ignore<ChargeModTemplate>();

            // ─── Seed your Classes ──────────────────────────────────────────────────
            modelBuilder.Entity<DestinyClass>().HasData(
                new DestinyClass { Id = 1, Name = "Warlock" },
                new DestinyClass { Id = 2, Name = "Hunter"  },
                new DestinyClass { Id = 3, Name = "Titan"   }
            );

            // ─── Seed your Subclasses ───────────────────────────────────────────────
            modelBuilder.Entity<Subclass>().HasData(
                new Subclass { Id =  1, DestinyClassId = 1, Name = "Dawnblade"        },
                new Subclass { Id =  2, DestinyClassId = 1, Name = "Voidwalker"       },
                new Subclass { Id =  3, DestinyClassId = 1, Name = "Stormcaller"      },
                new Subclass { Id =  4, DestinyClassId = 1, Name = "Shadebinder"      },
                new Subclass { Id =  5, DestinyClassId = 1, Name = "Broodweaver"      },
                new Subclass { Id =  6, DestinyClassId = 1, Name = "Prismatic Warlock"},


                new Subclass { Id =  7, DestinyClassId = 2, Name = "Gunslinger"       },
                new Subclass { Id =  8, DestinyClassId = 2, Name = "Nightstalker"     },
                new Subclass { Id =  9, DestinyClassId = 2, Name = "Arcstrider"       },
                new Subclass { Id = 10, DestinyClassId = 2, Name = "Revenant"         },
                new Subclass { Id = 11, DestinyClassId = 2, Name = "Threadrunner"     },
                new Subclass { Id = 12, DestinyClassId = 2, Name = "Prismatic Hunter" },


                new Subclass { Id = 13, DestinyClassId = 3, Name = "Sunbreaker"       },
                new Subclass { Id = 14, DestinyClassId = 3, Name = "Sentinel"         },
                new Subclass { Id = 15, DestinyClassId = 3, Name = "Striker"          },
                new Subclass { Id = 16, DestinyClassId = 3, Name = "Behemoth"         },
                new Subclass { Id = 17, DestinyClassId = 3, Name = "Berserker"        },
                new Subclass { Id = 18, DestinyClassId = 3, Name = "Prismatic Titan"  }
            );

            // ─── Seed your FocusOptions ────────────────────────────────────────────
            modelBuilder.Entity<FocusOption>().HasData(
                new FocusOption { Id = 1, Name = "Weapon Damage" },
                new FocusOption { Id = 2, Name = "Weapon Uptime" },
                new FocusOption { Id = 3, Name = "Ability Uptime"}
            );

            // ─── Seed your ChargeTypes ─────────────────────────────────────────────
            modelBuilder.Entity<ChargeType>().HasData(
                new ChargeType { Id = 1, Name = "Grenade"       },
                new ChargeType { Id = 2, Name = "Melee"         },
                new ChargeType { Id = 3, Name = "Class Ability" },
                new ChargeType { Id = 4, Name = "Super"         }
            );

            // ─── BuildMapping → ChargeType foreign key ─────────────────────────────
            modelBuilder.Entity<BuildMapping>()
                .HasOne(m => m.ChargeType)
                .WithMany()
                .HasForeignKey(m => m.ChargeTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── BuildMapping ↔ Aspect many-to-many ────────────────────────────────
            modelBuilder.Entity<BuildMapping>()
                .HasMany(b => b.Aspects)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "MappingAspects",
                    j => j
                        .HasOne<Aspect>()
                        .WithMany()
                        .HasForeignKey("AspectId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<BuildMapping>()
                        .WithMany()
                        .HasForeignKey("BuildMappingId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("BuildMappingId", "AspectId");
                        j.ToTable("MappingAspects");
                    }
                );

            // ─── BuildMapping ↔ Fragment many-to-many ───────────────────────────────
            modelBuilder.Entity<BuildMapping>()
                .HasMany(b => b.Fragments)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    "MappingFragments",
                    j => j
                        .HasOne<Fragment>()
                        .WithMany()
                        .HasForeignKey("FragmentId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<BuildMapping>()
                        .WithMany()
                        .HasForeignKey("BuildMappingId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("BuildMappingId", "FragmentId");
                        j.ToTable("MappingFragments");
                    }
                );

            // ─── Primary keys for Aspect & Fragment ───────────────────────────────
            modelBuilder.Entity<Aspect>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Fragment>()
                .HasKey(f => f.Hash);

            base.OnModelCreating(modelBuilder);
        }
    }
}
