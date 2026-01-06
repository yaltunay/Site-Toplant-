using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplanti.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiverPhoneToProxy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverPhone",
                table: "Proxies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverPhone",
                table: "Proxies");
        }
    }
}
