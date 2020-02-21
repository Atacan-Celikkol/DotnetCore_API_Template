using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace WebJobs
{
    public class BaseFunction
    {
        protected T GetModel<T>(Message message)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(message.Body));
        }
    }
}