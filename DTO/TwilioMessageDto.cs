using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.DTO
{
	public class TwilioMessageDto
	{

		[Required]
		[MaxLength(13)]
		[MinLength(11)]
        public string PhoneNumber { get; set; }

		[Required]
		[MinLength(1)]
        public string Message { get; set; }

    }
}
