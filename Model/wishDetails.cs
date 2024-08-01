using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWepApi.Model
{
	public class wishDetails
	{
		public int Id { get; set; }

		[ForeignKey("Wishlist")]
		public int? wishlistId { get; set; }

		public Wishlist? Wishlist { get; set; }

		[ForeignKey("product")]
		public int? productId { get; set; }

		public Product? product { get; set; }
	}
}
