using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcommerceWepApi.Migrations
{
    /// <inheritdoc />
    public partial class createOrderWithOrderDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ordersDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ordersDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ordersDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        onDelete:ReferentialAction.SetNull,
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ordersDetails_orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "orders",
						onDelete: ReferentialAction.SetNull,
						principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ordersDetails_OrderId",
                table: "ordersDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ordersDetails_ProductId",
                table: "ordersDetails",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ordersDetails");

            migrationBuilder.DropTable(
                name: "orders");
        }
    }
}
