using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Providers.Mail
{
    public class MailTrapMailProvider : IMailProvider
    {
        private readonly SmtpClient _client;

        public MailTrapMailProvider(string username, string password)
        {
            _client = new SmtpClient
            {
                Host = "mailtrap.io",
                Port = 2525, // check port with Mailtrap settings
                Credentials = new NetworkCredential(username, password), // credentials for mailtrap inbox
                EnableSsl = true,
            };
        }

        public async Task SendAsync(string email, string subject, string textBody = null, string htmlBody = null)
        {
            await SendAsync(new[] { email }, subject, textBody, htmlBody);
        }

        public async Task SendAsync(IEnumerable<string> emails, string subject, string textBody = null, string htmlBody = null)
        {
            if (htmlBody == null) return;
            foreach (var email in emails)
            {
                var mail = new MailMessage("no-reply@ovidos.com", email)
                {
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true,
                };
                await _client.SendMailAsync(mail);
            }
        }

        public Task SendAsync(string emailAddress, string name, string templateId, object templateData)
        {
            throw new System.NotImplementedException();
        }
    }
}