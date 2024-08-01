using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWepApi.Model
{
	public class Product
	{
		[Key]
        public int Id { get; set; }

		[Required]
        public string Description { get; set; }

		[Required]
        public int Quantity { get; set; }

		[Required]
        public string Title { get; set; }

        public int? Reviews { get; set; }

		[Required]
        public double Price { get; set; }

		public double? Discount { get; set; }

        public string[]? Imgs { get; set; }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }

        public Category? Category { get; set; }

		List<TrendingDetails> TrendingDetails { get; set; }

		List<cartDetails> cartDetails { get; set; }

		List<wishDetails> wishDetails { get; set; }

		List<OrderDetails> OrderDetails { get; set; }

	}
}
