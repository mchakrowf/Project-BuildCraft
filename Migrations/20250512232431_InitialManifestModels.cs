using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBuildCraft.Migrations
{
    /// <inheritdoc />
    public partial class InitialManifestModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_Classes_ClassId",
                table: "Mappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.RenameTable(
                name: "Classes",
                newName: "DestinyClass");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DestinyClass",
                table: "DestinyClass",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Archetypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Archetypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChargeTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exotics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exotics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SandboxPerks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SandboxPerks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ArchetypeModTemplates",
                columns: table => new
                {
                    ArchetypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Slot = table.Column<string>(type: "TEXT", nullable: false),
                    ModName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchetypeModTemplates", x => new { x.ArchetypeId, x.Slot });
                    table.ForeignKey(
                        name: "FK_ArchetypeModTemplates_Archetypes_ArchetypeId",
                        column: x => x.ArchetypeId,
                        principalTable: "Archetypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityArchetypes",
                columns: table => new
                {
                    EntityId = table.Column<int>(type: "INTEGER", nullable: false),
                    EntityType = table.Column<string>(type: "TEXT", nullable: false),
                    ArchetypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityArchetypes", x => new { x.EntityType, x.EntityId, x.ArchetypeId });
                    table.ForeignKey(
                        name: "FK_EntityArchetypes_Archetypes_ArchetypeId",
                        column: x => x.ArchetypeId,
                        principalTable: "Archetypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChargeModTemplates",
                columns: table => new
                {
                    ChargeTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Slot = table.Column<string>(type: "TEXT", nullable: false),
                    ModName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChargeModTemplates", x => new { x.ChargeTypeId, x.Slot });
                    table.ForeignKey(
                        name: "FK_ChargeModTemplates_ChargeTypes_ChargeTypeId",
                        column: x => x.ChargeTypeId,
                        principalTable: "ChargeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Subclasses",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Prismatic Warlock");

            migrationBuilder.UpdateData(
                table: "Subclasses",
                keyColumn: "Id",
                keyValue: 12,
                column: "Name",
                value: "Prismatic Hunter");

            migrationBuilder.UpdateData(
                table: "Subclasses",
                keyColumn: "Id",
                keyValue: 18,
                column: "Name",
                value: "Prismatic Titan");

            migrationBuilder.CreateIndex(
                name: "IX_EntityArchetypes_ArchetypeId",
                table: "EntityArchetypes",
                column: "ArchetypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_DestinyClass_ClassId",
                table: "Mappings",
                column: "ClassId",
                principalTable: "DestinyClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_DestinyClass_ClassId",
                table: "Mappings");

            migrationBuilder.DropTable(
                name: "ArchetypeModTemplates");

            migrationBuilder.DropTable(
                name: "ChargeModTemplates");

            migrationBuilder.DropTable(
                name: "EntityArchetypes");

            migrationBuilder.DropTable(
                name: "Exotics");

            migrationBuilder.DropTable(
                name: "SandboxPerks");

            migrationBuilder.DropTable(
                name: "ChargeTypes");

            migrationBuilder.DropTable(
                name: "Archetypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DestinyClass",
                table: "DestinyClass");

            migrationBuilder.RenameTable(
                name: "DestinyClass",
                newName: "Classes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Subclasses",
                keyColumn: "Id",
                keyValue: 6,
                column: "Name",
                value: "Prismatic");

            migrationBuilder.UpdateData(
                table: "Subclasses",
                keyColumn: "Id",
                keyValue: 12,
                column: "Name",
                value: "Prismatic");

            migrationBuilder.UpdateData(
                table: "Subclasses",
                keyColumn: "Id",
                keyValue: 18,
                column: "Name",
                value: "Prismatic");

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_Classes_ClassId",
                table: "Mappings",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
