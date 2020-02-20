using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace Providers.Queue
{
    public class AzureQueueProvider : IQueueProvider
    {
        private readonly CloudQueueClient _queueClient;

        public AzureQueueProvider(string connString)
        {
            if (string.IsNullOrEmpty(connString))
            {
                throw new ArgumentNullException(nameof(connString));
            }

            var conString = connString;
            var storageAccount = CloudStorageAccount.Parse(conString
            );
            _queueClient = storageAccount.CreateCloudQueueClient();


        }
        public async Task SendQueueAsync(string queueName, object data)
        {
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(data));
            var queue = _queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            await queue.AddMessageAsync(queueMessage);
        }

        public async Task SendQueueAsync(string queueName, object data, int delayInSeconds)
        {
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(data));
            var queue = _queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
            await queue.AddMessageAsync(queueMessage, null, TimeSpan.FromSeconds(delayInSeconds), null, null);
        }
    }
}
