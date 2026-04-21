using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawsella.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFragileToOrderPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Package_IsFragile",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Package_IsFragile",
                table: "Orders");
        }
    }
}
