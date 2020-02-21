using System.Collections.Generic;
using System.Threading.Tasks;

namespace Providers.Sms
{
    public interface ISmsProvider
    {
        Task SendAsync(string to, string from, string message);

        Task SendAsync(IEnumerable<string> to, string from, string message);
    }
}