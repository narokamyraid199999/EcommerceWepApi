using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using EcommerceWepApi.Model;

namespace EcommerceWepApi.DTO
{
	public class productDTo
	{
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

		public int? CategoryId { get; set; }

		public string? Category { get; set; }
	}
}
