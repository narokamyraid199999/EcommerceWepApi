using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWepApi.Model
{
	public class Order
	{
		[Key]
        public int Id { get; set; }
		public string status { get; set; } = "Pending";

		[ForeignKey("IdentityUser")]
		public string UserId { get; set; }

		public IdentityUser IdentityUser { get; set; }

		List<OrderDetails> OrderDetails { get; set; }

	}
}
