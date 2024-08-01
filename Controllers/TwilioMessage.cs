using EcommerceWepApi.DTO;
using EcommerceWepApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWepApi.Controllers
{
	[Route("api/twilioMessage")]
	[ApiController]
	public class TwilioMessage : ControllerBase
	{

		public TwilioMessage(ITwillioService twilioService) 
		{
			_twilioService = twilioService;
		}

		private readonly ITwillioService _twilioService;

		[HttpPost]
		public async Task<IActionResult> Send(TwilioMessageDto newTwilioMessage)
		{

			if (ModelState.IsValid)
			{
				if (!newTwilioMessage.PhoneNumber.Contains("+20"))
				{
					newTwilioMessage.PhoneNumber = $"+2{newTwilioMessage.PhoneNumber}";
				}
				try
				{
					var res = _twilioService.sendMessageAsync(newTwilioMessage.PhoneNumber, newTwilioMessage.Message);
					return Ok(new {msg= "The message has been sent"});
				}
				catch (Exception ex)
				{
					return BadRequest(new {error=$"{ex.Message}"});
				}

			}

			return BadRequest(ModelState);

		}

	}
}
