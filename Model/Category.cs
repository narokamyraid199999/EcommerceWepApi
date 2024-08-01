using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.Model
{
	public class Category
	{
		[Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string Name { get; set; } 

        public string? Image { get; set; }

		public List<Product> products { get; set; }

    }
}
