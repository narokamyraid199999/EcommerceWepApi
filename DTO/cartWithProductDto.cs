using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.DTO
{
	public class cartWithProductDto
	{		
		[Required]
		public int productId { get; set; }

		public int Quantity { get; set; } = 1;
    }
}
