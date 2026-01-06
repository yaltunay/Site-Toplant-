using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplanti.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitContactFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Units",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Units",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Units",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Units",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Units");
        }
    }
}
