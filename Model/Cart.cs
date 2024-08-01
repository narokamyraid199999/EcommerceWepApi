using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcommerceWepApi.Model
{
	public class Cart
	{
		[Key]
        public int Id { get; set; }

        [ForeignKey("IdentityUser")]
        public string UserId { get; set; }

        public IdentityUser IdentityUser { get; set; }

		List<cartDetails> cartDetails { get; set; }

	}
}
