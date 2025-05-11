using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectBuildCraft.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Armors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DestinyClassId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Armors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FocusOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FocusOptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subclasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    DestinyClassId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subclasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weapons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weapons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClassId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubclassId = table.Column<int>(type: "INTEGER", nullable: false),
                    ArmorId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeaponId = table.Column<int>(type: "INTEGER", nullable: false),
                    FocusOptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    PrimaryStatId = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondaryStatId = table.Column<int>(type: "INTEGER", nullable: false),
                    Mods = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mappings_Armors_ArmorId",
                        column: x => x.ArmorId,
                        principalTable: "Armors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mappings_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mappings_FocusOptions_FocusOptionId",
                        column: x => x.FocusOptionId,
                        principalTable: "FocusOptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mappings_Stats_PrimaryStatId",
                        column: x => x.PrimaryStatId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mappings_Stats_SecondaryStatId",
                        column: x => x.SecondaryStatId,
                        principalTable: "Stats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Mappings_Subclasses_SubclassId",
                        column: x => x.SubclassId,
                        principalTable: "Subclasses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mappings_Weapons_WeaponId",
                        column: x => x.WeaponId,
                        principalTable: "Weapons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Armors",
                columns: new[] { "Id", "DestinyClassId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Sunbracers" },
                    { 2, 1, "Transversive Steps" },
                    { 3, 2, "Celestial Nighthawk" },
                    { 4, 2, "Raiden Flux" },
                    { 5, 3, "Heart of Inmost Light" },
                    { 6, 3, "Dunemarchers" }
                });

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Warlock" },
                    { 2, "Hunter" },
                    { 3, "Titan" }
                });

            migrationBuilder.InsertData(
                table: "FocusOptions",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Weapon Damage" },
                    { 2, "Weapon Uptime" },
                    { 3, "Ability Uptime" }
                });

            migrationBuilder.InsertData(
                table: "Stats",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Mobility" },
                    { 2, "Resilience" },
                    { 3, "Recovery" },
                    { 4, "Discipline" },
                    { 5, "Intellect" },
                    { 6, "Strength" }
                });

            migrationBuilder.InsertData(
                table: "Subclasses",
                columns: new[] { "Id", "DestinyClassId", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Dawnblade" },
                    { 2, 1, "Voidwalker" },
                    { 3, 1, "Stormcaller" },
                    { 4, 1, "Shadebinder" },
                    { 5, 1, "Broodweaver" },
                    { 6, 1, "Prismatic" },
                    { 7, 2, "Gunslinger" },
                    { 8, 2, "Nightstalker" },
                    { 9, 2, "Arcstrider" },
                    { 10, 2, "Revenant" },
                    { 11, 2, "Threadrunner" },
                    { 12, 2, "Prismatic" },
                    { 13, 3, "Sentinel" },
                    { 14, 3, "Sunbreaker" },
                    { 15, 3, "Striker" },
                    { 16, 3, "Behemoth" },
                    { 17, 3, "Berserker" },
                    { 18, 3, "Prismatic" }
                });

            migrationBuilder.InsertData(
                table: "Weapons",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Dragon's Breath" },
                    { 2, "Ace of Spades" },
                    { 3, "Thorn" }
                });

            migrationBuilder.InsertData(
                table: "Mappings",
                columns: new[] { "Id", "ArmorId", "ClassId", "FocusOptionId", "Mods", "PrimaryStatId", "SecondaryStatId", "SubclassId", "Summary", "WeaponId" },
                values: new object[] { 2, 1, 1, 3, "Aspects: Touch of Flame; Fragments: Cure, Mercy, Flame, Searing; Helmet: Ashes to Assets ×2, Siphon Mod; Gloves: Firepower ×2, Momentum Transfer; Chest: Concussive Dampener, Emergency Reinforcement; Legs: Absolution, Restoration, Invigoration; Class Item: Powerful Attraction, Outreach, Distribution", 3, 6, 1, "Dawnblade build centered on Ability Uptime with Touch of Flame + high-regen fragments and Dragon's Breath.", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Mappings_ArmorId",
                table: "Mappings",
                column: "ArmorId");

            migrationBuilder.CreateIndex(
                name: "IX_Mappings_ClassId",
                table: "Mappings",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Mappings_FocusOptionId",
                table: "Mappings",
                column: "FocusOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Mappings_PrimaryStatId",
                table: "Mappings",
                column: "PrimaryStatId");

            migrationBuilder.CreateIndex(
                name: "IX_Mappings_SecondaryStatId",
                table: "Mappings",
                column: "SecondaryStatId");

            migrationBuilder.CreateIndex(
                name: "IX_Mappings_SubclassId",
                table: "Mappings",
                column: "SubclassId");

            migrationBuilder.CreateIndex(
                name: "IX_Mappings_WeaponId",
                table: "Mappings",
                column: "WeaponId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mappings");

            migrationBuilder.DropTable(
                name: "Armors");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "FocusOptions");

            migrationBuilder.DropTable(
                name: "Stats");

            migrationBuilder.DropTable(
                name: "Subclasses");

            migrationBuilder.DropTable(
                name: "Weapons");
        }
    }
}
