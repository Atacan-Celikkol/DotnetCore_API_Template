using OviPush.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Providers.PushNotification
{
    public class AzurePushNotificationProvider : IPushNotificationProvider
    {
        public AzurePushNotificationProvider(string hubName, string endpoint)
        {
            NotificationManager.SetConfiguration(hubName, endpoint);
        }

        public async Task SendNotificationAsync(string text, Dictionary<string, object> data = null, string badge = null, int count = 0, params string[] users)
        {
            if (users.Length > 0)
            {
                var hashUser = new HashSet<string>(users);
                await NotificationManager.Instance.SendNotificationAsync(text, data, hashUser.ToList(), badge, "default");
            }
            else
            {
                await NotificationManager.Instance.SendNotificationAsync(text, data, null, badge, "default");
            }
        }
    }
}