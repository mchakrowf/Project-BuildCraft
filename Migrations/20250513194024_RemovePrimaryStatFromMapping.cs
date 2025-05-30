using Microsoft.EntityFrameworkCore.Migrations;

namespace ProjectBuildCraft.Migrations
{
    public partial class RemovePrimaryStatFromMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key if it exists
            migrationBuilder.DropForeignKey(
                name: "FK_Mappings_Stats_PrimaryStatId",
                table: "Mappings");

            // Drop the index if it exists (SQLite supports IF EXISTS)
            migrationBuilder.Sql("DROP INDEX IF EXISTS \"IX_Mappings_PrimaryStatId\";");

            // Finally drop the column
            migrationBuilder.DropColumn(
                name: "PrimaryStatId",
                table: "Mappings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-add the column
            migrationBuilder.AddColumn<int>(
                name: "PrimaryStatId",
                table: "Mappings",
                type: "INTEGER",
                nullable: true);

            // Recreate the index
            migrationBuilder.CreateIndex(
                name: "IX_Mappings_PrimaryStatId",
                table: "Mappings",
                column: "PrimaryStatId");

            // Recreate the FK
            migrationBuilder.AddForeignKey(
                name: "FK_Mappings_Stats_PrimaryStatId",
                table: "Mappings",
                column: "PrimaryStatId",
                principalTable: "Stats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
