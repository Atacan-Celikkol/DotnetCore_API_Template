using System.Threading.Tasks;

namespace Providers.Queue
{
    public interface IQueueProvider
    {
        Task SendQueueAsync(string queueName, object data);
        Task SendQueueAsync(string queueName, object data, int delayInSeconds);
    }
}
