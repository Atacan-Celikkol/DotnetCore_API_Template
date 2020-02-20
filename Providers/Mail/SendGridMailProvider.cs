using System.Collections.Generic;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Providers.Mail
{
    public class SendGridMailProvider : IMailProvider
    {
        private readonly SendGridClient _client;
        private readonly string _sender;

        public SendGridMailProvider(string apiKey, string sender)
        {
            _client = new SendGridClient(apiKey);
            _sender = sender;
        }

        public async Task SendAsync(string emailAddress, string name, string templateId, object templateData)
        {
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress(_sender, "Atacan"));
            msg.AddTo(new EmailAddress(emailAddress, name));

            msg.SetTemplateId(templateId);
            msg.SetTemplateData(templateData);
            await _client.SendEmailAsync(msg);
        }

        public async Task SendAsync(string email, string subject, string textBody = null, string htmlBody = null)
        {
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_sender),
                Subject = subject,
                PlainTextContent = textBody,
                HtmlContent = htmlBody
            };
            msg.AddTo(new EmailAddress(email));
            // ReSharper disable once UnusedVariable
            var response = await _client.SendEmailAsync(msg);
        }

        public async Task SendAsync(IEnumerable<string> emails, string subject, string textBody = null, string htmlBody = null)
        {
            foreach (var mail in emails)
            {
                await SendAsync(mail, subject, textBody, htmlBody);
            }
        }
    }
}
