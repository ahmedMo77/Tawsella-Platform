using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tawsella.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceIdToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Orders");
        }
    }
}
