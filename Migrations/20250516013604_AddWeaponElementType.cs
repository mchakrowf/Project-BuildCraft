using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBuildCraft.Migrations
{
    /// <inheritdoc />
    public partial class AddWeaponElementType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ElementType",
                table: "Weapons",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Weapons",
                keyColumn: "Id",
                keyValue: 1,
                column: "ElementType",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Weapons",
                keyColumn: "Id",
                keyValue: 2,
                column: "ElementType",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Weapons",
                keyColumn: "Id",
                keyValue: 3,
                column: "ElementType",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElementType",
                table: "Weapons");
        }
    }
}
