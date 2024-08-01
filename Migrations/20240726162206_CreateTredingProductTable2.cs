using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWepApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateTredingProductTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrendingProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    isNew = table.Column<bool>(type: "bit", nullable: false),
                    isFeatured = table.Column<bool>(type: "bit", nullable: false),
                    isTopSellers = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrendingProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrendingDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    productId = table.Column<int>(type: "int", nullable: false),
                    TrendingProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrendingDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrendingDetails_Products_productId",
                        column: x => x.productId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_TrendingDetails_TrendingProducts_TrendingProductId",
                        column: x => x.TrendingProductId,
                        principalTable: "TrendingProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrendingDetails_productId",
                table: "TrendingDetails",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_TrendingDetails_TrendingProductId",
                table: "TrendingDetails",
                column: "TrendingProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrendingDetails");

            migrationBuilder.DropTable(
                name: "TrendingProducts");
        }
    }
}
