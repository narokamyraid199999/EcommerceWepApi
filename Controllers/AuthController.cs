using EcommerceWepApi.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using EcommerceWepApi.Helopers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EcommerceWepApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Mvc.Formatters;
using EcommerceWepApi.Model;
using Microsoft.Extensions.Options;

namespace EcommerceWepApi.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{

		public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> RoleManager, IConfiguration configRoot, IAuthService myAuthService, EcommerWepApiContext myDB, IMailService emailService, ITwillioService twilioService, IOptions<TwillioDTo> twilopDto)
		{
			_userManager = userManager;
			_roleManager = RoleManager;
			_config = configRoot;
			_authService = myAuthService;
			_myDB = myDB;
			_emailService = emailService;
			_twilioService = twilioService;
			_twilioDto = twilopDto.Value;
		}

		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IConfiguration _config;
		private readonly IAuthService _authService;
		private readonly IMailService _emailService;
		private readonly EcommerWepApiContext _myDB;
		private readonly ITwillioService _twilioService;
		private readonly TwillioDTo _twilioDto;


		public object QuiryHeloper { get; private set; }

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login(authData newAuthData)
		{
			if (ModelState.IsValid)
			{
				var userIdentity = await _userManager.FindByEmailAsync(newAuthData.Username);

				if (userIdentity != null)
				{

					if (!await _userManager.IsEmailConfirmedAsync(userIdentity))
					{
						return Unauthorized(new { error = $"Email {newAuthData.Username} is not confirmed" });
					}

					var passwordCheckRes = await _userManager.CheckPasswordAsync(userIdentity, newAuthData.Password);
					if (passwordCheckRes)
					{
						var tempUser = userIdentity;
						var userRoles = await _userManager.GetRolesAsync(tempUser);

						List<string> tempUserRoles = new List<string>();
						foreach (var role in userRoles)
						{
							tempUserRoles.Add(role);
						}

						var token = await _authService.createJwtToken(tempUser);
						return Ok(new { user = new AuthDto { Username = tempUser.UserName, Email = tempUser.Email, Roles = tempUserRoles, ExpireDate = token.ValidTo, isAuthenticated = true, Token = new JwtSecurityTokenHandler().WriteToken(token) } });
					}
					else
					{
						return BadRequest(new { error = $"{newAuthData.Password} is invalid password." });
					}
				}
				else
				{
					return BadRequest(new { error = $"{newAuthData.Username} is invalid username." });
				}
			}
			return BadRequest(ModelState);
		}


		[HttpPost]
		[Route("regester")]
		public async Task<IActionResult> regester(authData signUpData)
		{
			if (ModelState.IsValid)
			{

				var isRegesteredWithEmail = await _userManager.FindByEmailAsync(signUpData.Username);
				var isRegesteredWithUsername = await _userManager.FindByNameAsync(signUpData.Username);

				if (isRegesteredWithEmail != null || isRegesteredWithUsername != null) {
					return BadRequest(new { error = $"User {signUpData.Username} is already regesterd!" });
				}

				IdentityUser myIdentityUser = new IdentityUser { UserName = signUpData.Username, Email = signUpData.Username, PhoneNumber = signUpData.PhoneNumber };
				IdentityResult res = await _userManager.CreateAsync(myIdentityUser, signUpData.Password);
				if (res.Succeeded)
				{
					var roleExists = await _roleManager.RoleExistsAsync("User");
					if (roleExists)
					{
						var roleRegesterRes = await _userManager.AddToRoleAsync(myIdentityUser, "User");
						if (roleRegesterRes.Succeeded)
						{
							//var token = await _authService.createJwtToken(await _userManager.FindByNameAsync(signUpData.Username));

							// create cart for new user



							var emailVerifyToken = await _userManager.GenerateEmailConfirmationTokenAsync(myIdentityUser);

							var emailConfirmUrl = new Dictionary<string, string?>();

							emailConfirmUrl.Add("email", myIdentityUser.Email);
							emailConfirmUrl.Add("token", emailVerifyToken);

							string ConfirmLink = Url.Link("EmailConfirmationWithToken", null);

							var emailConfirmationQuiry = QueryHelpers.AddQueryString(ConfirmLink, emailConfirmUrl);

							await _emailService.sendMailAsync(myIdentityUser.Email, "Email Confirmation Token", emailConfirmationQuiry);
							try
							{
								var twilioRes = _twilioService.sendMessageAsync($"+2{myIdentityUser.PhoneNumber}", emailConfirmationQuiry);

								if (!string.IsNullOrEmpty(twilioRes.ErrorMessage))
								{
									return BadRequest(new { error = twilioRes.ErrorMessage });
								}
							} catch (Exception ex)
							{
								return BadRequest(ex.Message);
							}

							_myDB.Carts.Add(new Cart { UserId = myIdentityUser.Id });

							_myDB.Wishlists.Add(new Wishlist { UserId = myIdentityUser.Id });

							_myDB.SaveChanges();

							return Ok(new { user = signUpData, isRegesterd = true, emailComfirmed = false });

							//return Ok(new { user=signUpData, isRegesterd=true, emailComfirmed=false, emailConfirmUrl=emailConfirmationQuiry });
						}
						else
						{
							return BadRequest(new { err = $"Faild to create role for the user {signUpData}" });
						}
					}
					else
					{
						return BadRequest(new { error = "Role User does not exsits" });
					}

				}
				else
				{
					foreach (var err in res.Errors)
					{
						ModelState.AddModelError("", err.Description);
					}
					return BadRequest(ModelState);
				}
			}
			return BadRequest(ModelState);
		}


		[HttpGet("users")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> getAllUsers()
		{
			var users = await _userManager.Users.ToListAsync();
			List<UserDto> UsersDto = new List<UserDto>();

			foreach (var user in users) {

				var roles = await _userManager.GetRolesAsync(user);

				UsersDto.Add(new UserDto { Id = user.Id, Password = user.PasswordHash, Username = user.UserName, Roles = roles });
			};

			string key = _config.GetSection("JWT:key").Value;


			return Ok(new { result = UsersDto, status = "success" });
		}

		[HttpPost("addRoleToUser")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> addRoleToUser([FromQuery] string username, [FromQuery] string roleName)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(username);
				if (user != null)
				{
					if (await _roleManager.RoleExistsAsync(roleName))
					{
						IdentityResult res = await _userManager.AddToRoleAsync(user, roleName);
						if (res.Succeeded)
						{
							return Ok(new { msg = $"role {roleName} has been assigned to user {username}" });
						}
						else
						{
							foreach (var error in res.Errors)
							{
								ModelState.AddModelError("", error.Description);
							}
							return BadRequest(ModelState);
						}
					}
					else
					{
						return BadRequest(new { error = $"Role {roleName} is invalid" });
					}
				}
				else
				{
					return BadRequest(new { error = $"User {username} is invalid" });
				}
			}
			return BadRequest(ModelState);
		}

		[HttpGet("user/{email}/roles")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> getRoleForUser([FromRoute] string email)
		{

			var user = await _userManager.FindByEmailAsync(email);

			if (user != null)
			{
				var roles = await _userManager.GetRolesAsync(user);

				return Ok(new { user = email, roles = roles, status = "success" });
			}

			return BadRequest(new { error = $"email {email} is invalid." });

		}

		[HttpDelete("user/{email}/removeRole")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> removeUserFromRole([FromRoute] string email, [FromQuery] string roleName)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(email);
				if (user != null)
				{
					if (await _roleManager.RoleExistsAsync(roleName))
					{
						var res = await _userManager.GetRolesAsync(user);
						if (res.Contains(roleName))
						{
							IdentityResult roleState = await _userManager.RemoveFromRoleAsync(user, roleName);
							if (roleState.Succeeded)
							{
								return Ok(new { msg = $"role {roleName} has been removed from user {email}" });
							}
							else
							{
								foreach (var error in roleState.Errors)
								{
									ModelState.AddModelError("", error.Description);
								}

								return BadRequest(ModelState);
							}
						}
						else
						{
							return BadRequest(new { error = $"User {email} doesn't have role {roleName}" });
						}
					}
					else
					{
						return BadRequest(new { error = $"Role {roleName} is invalid" });
					}
				}
				else
				{
					return BadRequest(new { error = $"User {email} is invalid" });
				}
			}
			return BadRequest(ModelState);
		}

		[HttpGet]
		[Route("emailConfirmation", Name = "EmailConfirmationWithToken")]
		public async Task<IActionResult> emailConfirm([FromQuery] string token, [FromQuery] string email)
		{

			var tempUser = await _userManager.FindByEmailAsync(email);

			if (tempUser == null)
			{
				return BadRequest(new { error = "Invalid email address" });
			}

			if (token == null)
			{
				return BadRequest(new { error = "Invalid token" });
			}

			var tokenChecker = await _userManager.ConfirmEmailAsync(tempUser, token);

			if (tokenChecker.Succeeded)
			{
				return Ok(new { msg = $"email {email} has been verified" });
			}
			else
			{
				string errorMsg = "";
				foreach (var error in tokenChecker.Errors)
				{
					errorMsg += error.Description + ", ";
				}

				ModelState.AddModelError("error", errorMsg);

				return BadRequest(ModelState);
			}

		}

		[HttpGet("sendEmailConfirmation", Name = "confirmEmailAddressByToken")]
		public async Task<IActionResult> sendEmailConfirmation([FromQuery] string email)
		{
			var tempUser = await _userManager.FindByEmailAsync(email);

			if (tempUser == null)
			{
				return Unauthorized(new { error = "Invalid email address" });
			}

			if (await _userManager.IsEmailConfirmedAsync(tempUser))
			{
				return BadRequest(new { error = $"Email {email} is already confirmed!" });
			}

			var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(tempUser);

			var confirmUrl = QueryHelpers.AddQueryString(_config.GetSection("emailConfirmUrl").Value, new Dictionary<string, string?> { { "token", confirmationToken }, { "email", tempUser.Email } });

			await _emailService.sendMailAsync(email, "Email Confirmation Token", confirmUrl);

			return Ok(new { details = new { email = $"{email}", msg = $"Please check your email inbox for confirmation message" }, status = "success" });
		}

		[HttpGet("forgetPassword")]
		[Authorize]
		public async Task<IActionResult> forgetPassword([FromQuery] string newPassword)
		{

			var userId = User.Claims.FirstOrDefault(x => x.Type == "userId").Value;


			var tempUser = await _userManager.FindByIdAsync(userId);

			string userEmail = tempUser.Email;


			if (tempUser == null)
			{
				return BadRequest(new { errpr = "Invaild user" });
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(tempUser);

			string confirmUrl = Url.Link("userPasswordReset", null);

			string passwordResetUrl = QueryHelpers.AddQueryString(confirmUrl, new Dictionary<string, string?> { { "token", token }, { "email", userEmail }, { "newPassword", newPassword } });

			await _emailService.sendMailAsync(userEmail, "Password reset token", passwordResetUrl);

			return Ok(new {msg="Please check your email address for reset token"});
		}

		[HttpGet("passwordResetConfirm", Name ="userPasswordReset")]
		public async Task<IActionResult> passwordTokenConfirm([FromQuery] string email, [FromQuery] string token, [FromQuery] string newPassword)
		{

			var tempUser = await _userManager.FindByEmailAsync(email);
			if (tempUser == null)
			{
				return BadRequest(new { errpr = "Invaild email address" });
			}

		    IdentityResult res = await _userManager.ResetPasswordAsync(tempUser, token, newPassword);

			if (res.Succeeded)
			{
				return Ok(new {msg="Password has been changed succfully"});
			}
			else
			{
				string errorMsg = "";
				foreach(var error in res.Errors)
				{
					errorMsg += error.Description + ", ";
				}

				ModelState.AddModelError("Error", errorMsg);

				return BadRequest(ModelState);
			}

		}

	}
}
