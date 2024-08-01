using EcommerceWepApi.Model;
using EcommerceWepApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EcommerceWepApi.Controllers
{
	[Route("api/contacts")]
	[ApiController]
	public class ContactsController : ControllerBase
	{


		public ContactsController(IContact contactService, UserManager<IdentityUser> userManager) 
		{
			_contactService = contactService;
			_userManager = userManager;
		}

		private readonly IContact _contactService;
		private readonly UserManager<IdentityUser> _userManager;

		[HttpGet]
		[Authorize(Roles ="Admin")]
		public async Task<IActionResult> getAll()
		{
			var contacts = await _contactService.getAll();
			
			var tempData = User.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.Email);
			var userId = User.Claims.FirstOrDefault(x => x.Type.Contains("userId"));
			var user = await _userManager.FindByIdAsync(userId.Value);

			return Ok(contacts);
		}

		[HttpPost]
		public async Task<IActionResult> Create(Contact newContact)
		{
			if (ModelState.IsValid)
			{
				var tempContact = await _contactService.Create(newContact);
				if (tempContact != null)
				{
					return Ok(new {details=tempContact, status="success"});
				}
				return BadRequest(new { msg = "Faild to create new contact" });

			}
			return BadRequest(ModelState);
		}

		[HttpDelete("{id}")]
		[Authorize(Roles ="Admin")]
		public async Task<IActionResult> Delete([FromRoute] int id)
		{
			bool delRes = await _contactService.Delete(id);
			if (delRes)
			{
				return NoContent();
			}

			return Unauthorized(new { error=$"Faild to delete contact id {id}"});

		}

	}
}
