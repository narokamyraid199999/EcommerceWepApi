using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWepApi.Model
{
	public class OrderDetails
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("Order")]
        public int? OrderId { get; set; }

        public Order? Order { get; set; }

		[ForeignKey("Product")]
		public int? ProductId { get; set; }

        public Product? Product { get; set; }

    }
}
