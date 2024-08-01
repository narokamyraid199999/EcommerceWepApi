using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWepApi.Model
{
	public class cartDetails
	{
        public int Id { get; set; }

        [ForeignKey("cart")]
        public int cartId { get; set; }

        public Cart cart { get; set; }

		[ForeignKey("product")]
		public int productId { get; set; }

		public Product product { get; set; }
	}
}
