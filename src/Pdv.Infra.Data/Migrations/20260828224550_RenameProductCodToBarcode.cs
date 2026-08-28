using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pdv.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameProductCodToBarcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Cod",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Cod",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "Products",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Barcode",
                table: "Products",
                column: "Barcode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_Barcode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "Cod",
                table: "Products",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_Cod",
                table: "Products",
                column: "Cod",
                unique: true);
        }
    }
}
