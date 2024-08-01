using EcommerceWepApi.DTO;
using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.Model
{
	public class categoryDTo
	{
        public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public string Image { get; set; }

		public List<productDTo> Products { get; set; } = new List<productDTo>();

	}
}
