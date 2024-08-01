using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.Model
{
	public class Wishlist
	{
		[Key]
		public int Id { get; set; }

		[ForeignKey("IdentityUser")]
		public string UserId { get; set; }

		public IdentityUser IdentityUser { get; set; }

		List<wishDetails> wishDetails { get; set; }
	}
}
