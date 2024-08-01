using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWepApi.Model
{
	public class TrendingDetails
	{
		[Key]
        public int Id { get; set; }

		[ForeignKey("Product")]
		public int productId { get; set; }

		public Product Product { get; set; }

		[ForeignKey("TrendingProduct")]
		public int TrendingProductId { get; set; }

		public TrendingProduct TrendingProduct { get; set; }

	}
}
