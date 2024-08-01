using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace EcommerceWepApi.Services
{
	public interface IAuthService
	{
		Task<JwtSecurityToken> createJwtToken(IdentityUser myUser);
	}
}