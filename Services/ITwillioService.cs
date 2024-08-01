using Twilio.Rest.Api.V2010.Account;

namespace EcommerceWepApi.Services
{
	public interface ITwillioService
	{

		public MessageResource sendMessageAsync(string sendTo, string body);

	}
}
