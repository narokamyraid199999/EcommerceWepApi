using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.Model
{
	public class Contact
	{
		[Key]
        public int Id { get; set; }

		[Required]
		[MinLength(3)]
        public string Name { get; set; }

        [Required]
		[DataType(DataType.EmailAddress)]
        public string Email { get; set; }

		[Required]
		[MinLength(3)]
		public string Subject { get; set; }

		[Required]
		[MinLength(3)]
		public string Message { get; set; }

	}
}
