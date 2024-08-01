using System.ComponentModel.DataAnnotations;

namespace EcommerceWepApi.DTO
{
	public class RoleDto
	{
        [Required]
        [MinLength(2)]
        public string RoleName { get; set; }
    }
}
