using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeteriaWeb.Migrations
{
    /// <inheritdoc />
    public partial class updb8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_CategorySupplier_CategorySupplierId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Suppliers");

            migrationBuilder.AlterColumn<int>(
                name: "CategorySupplierId",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_CategorySupplier_CategorySupplierId",
                table: "Suppliers",
                column: "CategorySupplierId",
                principalTable: "CategorySupplier",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_CategorySupplier_CategorySupplierId",
                table: "Suppliers");

            migrationBuilder.AlterColumn<int>(
                name: "CategorySupplierId",
                table: "Suppliers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Suppliers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_CategorySupplier_CategorySupplierId",
                table: "Suppliers",
                column: "CategorySupplierId",
                principalTable: "CategorySupplier",
                principalColumn: "Id");
        }
    }
}
