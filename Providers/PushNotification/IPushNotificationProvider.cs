using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Providers.PushNotification
{
    public interface IPushNotificationProvider
    {
        Task SendNotificationAsync(string text, Dictionary<string, object> data = null, string badge = null, int count = 0, params string[] users);
    }
}
