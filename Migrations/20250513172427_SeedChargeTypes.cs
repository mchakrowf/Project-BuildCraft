using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjectBuildCraft.Migrations
{
    /// <inheritdoc />
    public partial class SeedChargeTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ChargeTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Grenade" },
                    { 2, "Melee" },
                    { 3, "Class Ability" },
                    { 4, "Super" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ChargeTypes",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
