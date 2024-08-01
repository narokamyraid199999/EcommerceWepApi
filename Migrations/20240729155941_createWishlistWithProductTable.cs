using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWepApi.Migrations
{
    /// <inheritdoc />
    public partial class createWishlistWithProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Wishlists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wishlists_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "WishDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    wishlistId = table.Column<int>(type: "int", nullable: true),
                    productId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishDetails_Products_productId",
                        column: x => x.productId,
                        principalTable: "Products",
						onDelete: ReferentialAction.SetNull,
						principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WishDetails_Wishlists_wishlistId",
                        column: x => x.wishlistId,
						onDelete: ReferentialAction.SetNull,
						principalTable: "Wishlists",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_WishDetails_productId",
                table: "WishDetails",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_WishDetails_wishlistId",
                table: "WishDetails",
                column: "wishlistId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlists_UserId",
                table: "Wishlists",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WishDetails");

            migrationBuilder.DropTable(
                name: "Wishlists");
        }
    }
}
