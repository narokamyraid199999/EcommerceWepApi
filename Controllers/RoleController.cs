using EcommerceWepApi.DTO;
using EcommerceWepApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWepApi.Controllers
{
	[Route("api/roles")]
	[ApiController]
	[Authorize(Roles = "Admin")]
	public class RolesController : ControllerBase
	{

		public RolesController(RoleManager<IdentityRole> roleManager) 
		{
			_roleManager=roleManager;
		}

		private readonly RoleManager<IdentityRole> _roleManager;

		[HttpGet]
		public async Task<IActionResult> getAll()
		{
			var roles = _roleManager.Roles.ToList();
			return Ok(new {result=roles, status="success"});
		}

		[HttpGet("{name}")]
		public async Task<IActionResult> getByName([FromRoute] string name)
		{
			var role = await _roleManager.FindByNameAsync(name);
			if (role != null)
			{
				return Ok(role);
			}
			return BadRequest(new { error = $"role id {name} is invalid" });
		}

		[HttpPost]
		public async Task<IActionResult> newRole([FromBody] RoleDto newRole)
		{
			if (ModelState.IsValid)
			{
				if (!await _roleManager.RoleExistsAsync(newRole.RoleName))
				{
					var temoNewRole = new IdentityRole(newRole.RoleName);
					await _roleManager.CreateAsync(temoNewRole);
					return Ok(new { msg = $"role {newRole.RoleName} has been succfully created" });
				}
				return BadRequest(new { msg = $"role {newRole.RoleName} is already exists" });
			}
			return BadRequest(ModelState);
		}

		[HttpDelete]
		[Route("{id}")]
		public async Task<IActionResult> Delete([FromRoute] string id)
		{
			var role = await _roleManager.FindByIdAsync(id);
			if (role != null)
			{
			   IdentityResult res =	await _roleManager.DeleteAsync(role);
				if (res.Succeeded)
				{
					return NoContent();
				}
				else
				{
					foreach(var error in res.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
					return BadRequest(ModelState);
				}
			}

			return BadRequest(new {error=$"role id {id} is invalid"});
		}

		[HttpDelete]
		[Route("name/{name}")]
		public async Task<IActionResult> nameDelete([FromRoute] string name) 
		{
			var role = await _roleManager.FindByNameAsync(name);
			if (role != null)
			{
				IdentityResult res = await _roleManager.DeleteAsync(role);
				if (res.Succeeded)
				{
					return NoContent();
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

			return BadRequest(new { error = $"role name {name} is invalid" });
		}

	}
}
