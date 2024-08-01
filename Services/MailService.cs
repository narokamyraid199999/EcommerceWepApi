
using EcommerceWepApi.Model;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EcommerceWepApi.Services
{
	public class MailService : IMailService
	{

		public MailService(IOptions<EmailConfiguration> emailConfiguration) 
		{
			_emailConfiguration = emailConfiguration.Value;
		}

		private readonly EmailConfiguration _emailConfiguration;

		public async Task<string?> sendMailAsync(string mailTo, string subject, string body)
		{

			var Message = new MimeMessage 
			{
				Sender=MailboxAddress.Parse(_emailConfiguration.Username),
				Subject=subject,
			};

			Message.To.Add(MailboxAddress.Parse(mailTo));

			var emailBodyBuilder = new BodyBuilder();

			emailBodyBuilder.HtmlBody = body;
			
			Message.Body = emailBodyBuilder.ToMessageBody();
			Message.From.Add(new MailboxAddress(_emailConfiguration.From, _emailConfiguration.Username));

			var smtp = new SmtpClient();
			smtp.Connect(_emailConfiguration.Smtp, _emailConfiguration.Port, true);
			smtp.AuthenticationMechanisms.Remove("XOAUTH2");
			smtp.Authenticate(_emailConfiguration.Username, _emailConfiguration.Password);

			var res = await smtp.SendAsync(Message);

			smtp.Disconnect(true);

			return res;

		}
	}
}
