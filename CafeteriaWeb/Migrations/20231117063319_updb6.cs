using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CafeteriaWeb.Migrations
{
    /// <inheritdoc />
    public partial class updb6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NormalPrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalPrice",
                table: "Products");
        }
    }
}
