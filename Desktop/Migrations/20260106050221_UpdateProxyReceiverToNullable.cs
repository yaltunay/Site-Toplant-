using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Toplanti.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProxyReceiverToNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ReceiverUnitId",
                table: "Proxies",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "Proxies",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "Proxies");

            migrationBuilder.AlterColumn<int>(
                name: "ReceiverUnitId",
                table: "Proxies",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
