using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWepApi.Migrations
{
    /// <inheritdoc />
    public partial class addImgsToProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Imgs",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Imgs",
                table: "Products");
        }
    }
}
