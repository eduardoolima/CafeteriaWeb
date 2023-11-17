using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeteriaWeb.Migrations
{
    /// <inheritdoc />
    public partial class updb4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Sale_SaleId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "SaleId",
                table: "Products",
                newName: "PromotionId");

            migrationBuilder.RenameColumn(
                name: "IsOnSale",
                table: "Products",
                newName: "IsOnPromotion");

            migrationBuilder.RenameIndex(
                name: "IX_Products_SaleId",
                table: "Products",
                newName: "IX_Products_PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Sale_PromotionId",
                table: "Products",
                column: "PromotionId",
                principalTable: "Sale",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Sale_PromotionId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "PromotionId",
                table: "Products",
                newName: "SaleId");

            migrationBuilder.RenameColumn(
                name: "IsOnPromotion",
                table: "Products",
                newName: "IsOnSale");

            migrationBuilder.RenameIndex(
                name: "IX_Products_PromotionId",
                table: "Products",
                newName: "IX_Products_SaleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Sale_SaleId",
                table: "Products",
                column: "SaleId",
                principalTable: "Sale",
                principalColumn: "Id");
        }
    }
}
