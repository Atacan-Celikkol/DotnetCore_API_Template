using System.Collections.Generic;
using System.Threading.Tasks;

namespace Providers.Mail
{
    public interface IMailProvider
    {
        Task SendAsync(string email, string subject, string textBody = null, string htmlBody = null);

        Task SendAsync(IEnumerable<string> emails, string subject, string textBody = null, string htmlBody = null);

        Task SendAsync(string emailAddress, string name, string templateId, object templateData);
    }
}