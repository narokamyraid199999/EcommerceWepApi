
using EcommerceWepApi.Model;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace EcommerceWepApi.Services
{
	public class TwillioService : ITwillioService
	{

		public TwillioService(IOptions<TwillioDTo> twilioDto) 
		{
			_twilioDto = twilioDto.Value;
		}

		private readonly TwillioDTo _twilioDto;

		public MessageResource sendMessageAsync(string sendTo, string body)
		{
			TwilioClient.Init(_twilioDto.accountSID, _twilioDto.authToken);

			var res = MessageResource.Create(
				from:new PhoneNumber(_twilioDto.phoneNumber), 
				to:new PhoneNumber(sendTo),
				body:body
			);

			return res;

		}
	}
}
