using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBuildCraft.Migrations
{
    /// <inheritdoc />
    public partial class IncludeChargeTypeInMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArchetypeModTemplates_Archetypes_ArchetypeId",
                table: "ArchetypeModTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityArchetypes_Archetypes_ArchetypeId",
                table: "EntityArchetypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_DestinyClass_ClassId",
                table: "Mappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_Stats_PrimaryStatId",
                table: "Mappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_Stats_SecondaryStatId",
                table: "Mappings");

            migrationBuilder.DropIndex(
                name: "IX_Mappings_PrimaryStatId",
                table: "Mappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DestinyClass",
                table: "DestinyClass");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Archetypes",
                table: "Archetypes");

            migrationBuilder.DropColumn(
                name: "PrimaryStatId",
                table: "Mappings");

            migrationBuilder.RenameTable(
                name: "DestinyClass",
                newName: "Classes");

            migrationBuilder.RenameTable(
                name: "Archetypes",
                newName: "Archetype");

            migrationBuilder.RenameColumn(
                name: "SecondaryStatId",
                table: "Mappings",
                newName: "ChargeTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Mappings_SecondaryStatId",
                table: "Mappings",
                newName: "IX_Mappings_ChargeTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Archetype",
                table: "Archetype",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Mappings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ChargeTypeId", "Summary" },
                values: new object[] { 2, "Sunbracers Melee-focused Dawnblade build with high ability uptime." });

            migrationBuilder.AddForeignKey(
                name: "FK_ArchetypeModTemplates_Archetype_ArchetypeId",
                table: "ArchetypeModTemplates",
                column: "ArchetypeId",
                principalTable: "Archetype",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityArchetypes_Archetype_ArchetypeId",
                table: "EntityArchetypes",
                column: "ArchetypeId",
                principalTable: "Archetype",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_ChargeTypes_ChargeTypeId",
                table: "Mappings",
                column: "ChargeTypeId",
                principalTable: "ChargeTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_Classes_ClassId",
                table: "Mappings",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArchetypeModTemplates_Archetype_ArchetypeId",
                table: "ArchetypeModTemplates");

            migrationBuilder.DropForeignKey(
                name: "FK_EntityArchetypes_Archetype_ArchetypeId",
                table: "EntityArchetypes");

            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_ChargeTypes_ChargeTypeId",
                table: "Mappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_Classes_ClassId",
                table: "Mappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Archetype",
                table: "Archetype");

            migrationBuilder.RenameTable(
                name: "Classes",
                newName: "DestinyClass");

            migrationBuilder.RenameTable(
                name: "Archetype",
                newName: "Archetypes");

            migrationBuilder.RenameColumn(
                name: "ChargeTypeId",
                table: "Mappings",
                newName: "SecondaryStatId");

            migrationBuilder.RenameIndex(
                name: "IX_Mappings_ChargeTypeId",
                table: "Mappings",
                newName: "IX_Mappings_SecondaryStatId");

            migrationBuilder.AddColumn<int>(
                name: "PrimaryStatId",
                table: "Mappings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DestinyClass",
                table: "DestinyClass",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Archetypes",
                table: "Archetypes",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Mappings",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PrimaryStatId", "SecondaryStatId", "Summary" },
                values: new object[] { 3, 6, "Dawnblade build centered on Ability Uptime with Touch of Flame + high-regen fragments and Dragon's Breath." });

            migrationBuilder.CreateIndex(
                name: "IX_Mappings_PrimaryStatId",
                table: "Mappings",
                column: "PrimaryStatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ArchetypeModTemplates_Archetypes_ArchetypeId",
                table: "ArchetypeModTemplates",
                column: "ArchetypeId",
                principalTable: "Archetypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EntityArchetypes_Archetypes_ArchetypeId",
                table: "EntityArchetypes",
                column: "ArchetypeId",
                principalTable: "Archetypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_DestinyClass_ClassId",
                table: "Mappings",
                column: "ClassId",
                principalTable: "DestinyClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_Stats_PrimaryStatId",
                table: "Mappings",
                column: "PrimaryStatId",
                principalTable: "Stats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_Stats_SecondaryStatId",
                table: "Mappings",
                column: "SecondaryStatId",
                principalTable: "Stats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
