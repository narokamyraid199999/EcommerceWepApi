using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EcommerceWepApi.Services
{
	public class AuthService : IAuthService
	{

		public AuthService(UserManager<IdentityUser> userManager, IConfiguration myConfig)
		{
			_userManager = userManager;
			_config = myConfig;
		}

		private readonly UserManager<IdentityUser> _userManager;
		private readonly IConfiguration _config;

		public async Task<JwtSecurityToken> createJwtToken(IdentityUser myUser)
		{
			var userClaims = await _userManager.GetClaimsAsync(myUser);
			var userRoles = await _userManager.GetRolesAsync(myUser);
			List<Claim> roles = new List<Claim>();

			foreach (var role in userRoles)
			{
				roles.Add(new Claim("roles", role));
			}

			var Claims = new Claim[] {
				new Claim(JwtRegisteredClaimNames.Sub, myUser.UserName),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Email, myUser.Email),
				new Claim("userId", myUser.Id)
			}.Union(userClaims).Union(roles);

			var symatricSecuritykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt:Key").Value));
			var signInCredintial = new SigningCredentials(symatricSecuritykey, SecurityAlgorithms.HmacSha256);

			var jwtToken = new JwtSecurityToken(

				issuer: _config.GetSection("JWT:Issuer").Value,
				audience: _config.GetSection("JWT:Audience").Value,
				claims: Claims,
				expires: DateTime.Now.AddDays(int.Parse(_config.GetSection("Jwt:DurationInDays").Value)),
				signingCredentials: signInCredintial

				);

			return jwtToken;
		}
	}
}
