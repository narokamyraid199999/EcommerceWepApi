namespace EcommerceWepApi.Services
{
	public interface IMailService
	{
		public Task<string?> sendMailAsync(string maailTo, string subject, string body);
	}
}
