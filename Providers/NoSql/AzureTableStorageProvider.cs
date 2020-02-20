using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Exceptions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Providers.NoSql;

namespace Services.Providers
{
    public class AzureTableStorageProvider : ITableStorageProvider
    {
        private readonly CloudTableClient _tableClient;

        public AzureTableStorageProvider(string connString)
        {
            if (string.IsNullOrEmpty(connString))
            {
                throw new ArgumentNullException(nameof(connString));
            }

            var conString = connString;
            var tableAccount = CloudStorageAccount.Parse(conString
            );
            _tableClient = tableAccount.CreateCloudTableClient();


        }

        public AzureTableStorageProvider(CloudTableClient client)
        {
            _tableClient = client;
        }

        public async Task<T> SaveAsync<T>(string tableName, T data) where T : TableEntity
        {
            var table = _tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            var insertOperation = TableOperation.Insert(data);
            var result = await table.ExecuteAsync(insertOperation);
            if (result.HttpStatusCode >= 200 && result.HttpStatusCode < 300)
            {
                return data;
            }
            throw new Exception("Error");
        }

        public async Task<T> GetAsync<T>(string tableName, string id) where T : TableEntity
        {
            var table = _tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            var retrieveOperation = TableOperation.Retrieve<T>(id, typeof(T).Name);
            var retrievedResult = await table.ExecuteAsync(retrieveOperation);
            return (T)retrievedResult.Result;
        }

        public async Task<T> GetAsync<T>(string tableName, string partitionKey, string rowKey) where T : TableEntity
        {
            var table = _tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            var retrieveOperation = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var retrievedResult = await table.ExecuteAsync(retrieveOperation);
            return (T)retrievedResult.Result;
        }

        public async Task<T> SaveOrReplaceAsync<T>(string tableName, T data) where T : TableEntity
        {
            var table = _tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            var insertOperation = TableOperation.InsertOrReplace(data);
            var result = await table.ExecuteAsync(insertOperation);
            if (result.HttpStatusCode >= 200 && result.HttpStatusCode < 300)
            {
                return data;
            }
            throw new Exception("Error");
        }

        public async Task<List<T>> WhereAsync<T>(string tableName, TableQuery<T> query) where T : TableEntity, new()
        {
            var table = _tableClient.GetTableReference(tableName);
            var entities = new List<T>();
            TableContinuationToken continuationToken = null;
            do
            {
                var result = await table.ExecuteQuerySegmentedAsync(query, continuationToken);
                entities.AddRange(result.ToList());
                continuationToken = result.ContinuationToken;

            } while (continuationToken != null);
            return entities;
        }
    }

    public class BaseTableEntity : TableEntity
    {
        public BaseTableEntity()
        {
            CreateDate = DateTimeOffset.UtcNow;
            RowKey = GetType().Name;
        }

        public BaseTableEntity(string rowKey, string partitionKey)
        {
            CreateDate = DateTimeOffset.UtcNow;
            RowKey = rowKey;
            PartitionKey = partitionKey;
        }

        public DateTimeOffset CreateDate { get; set; }
    }

}

