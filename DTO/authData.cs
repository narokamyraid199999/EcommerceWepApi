using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.DTO
{
	public class authData
	{

        [Required]
		[MinLength(3)]
		[MaxLength(50)]
        public string Username { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[MinLength(6)]
		[MaxLength(40)]
        public string Password { get; set; }

		[Required]
		[MaxLength(13)]
		[MinLength(11)]
		public string PhoneNumber { get; set; }
	}
}
