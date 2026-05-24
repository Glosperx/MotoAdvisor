using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MotoAdvisor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMotorcycleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Horsepower",
                table: "Motorcycles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsBeginnerFriendly",
                table: "Motorcycles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LicenseCategory",
                table: "Motorcycles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Horsepower",
                table: "Motorcycles");

            migrationBuilder.DropColumn(
                name: "IsBeginnerFriendly",
                table: "Motorcycles");

            migrationBuilder.DropColumn(
                name: "LicenseCategory",
                table: "Motorcycles");
        }
    }
}
