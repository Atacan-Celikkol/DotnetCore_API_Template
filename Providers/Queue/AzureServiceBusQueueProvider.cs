using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Providers.Queue
{
    public class AzureServiceBusQueueProvider : IQueueProvider
    {
        private readonly string _connectionString;
        public AzureServiceBusQueueProvider(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(_connectionString));
            }
            _connectionString = connectionString;
        }
        public async Task SendQueueAsync(string queueName, object data)
        {
            var client = new QueueClient(_connectionString, queueName);
            var payload = JsonConvert.SerializeObject(data);
            Message message = new Message(Encoding.UTF8.GetBytes(payload))
            {
                ContentType = "application/json"
            };
            await client.SendAsync(message);
        }

        public async Task SendQueueAsync(string queueName, object data, int delayInSeconds)
        {
            var client = new QueueClient(_connectionString, queueName);
            var payload = JsonConvert.SerializeObject(data);
            Message message = new Message(Encoding.UTF8.GetBytes(payload))
            {
                ScheduledEnqueueTimeUtc = DateTime.UtcNow.AddSeconds(delayInSeconds),
                ContentType = "application/json"
            };
            await client.SendAsync(message);

        }

        public void SendQueue(string queueName, object data)
        {
            var task = SendQueueAsync(queueName, data);
            task.Wait();
        }

        public void SendQueue(string queueName, object data, int delayInSeconds)
        {
            var task = SendQueueAsync(queueName, data, delayInSeconds);
            task.Wait();
        }
    }
    public class ScheduleQueueModel
    {
        public string Id { get; set; }
    }
}
