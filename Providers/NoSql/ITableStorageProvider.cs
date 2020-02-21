using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Providers.NoSql
{
    public interface ITableStorageProvider
    {
        Task<T> SaveAsync<T>(string tableName, T data) where T : TableEntity;

        Task<T> GetAsync<T>(string tableName, string id) where T : TableEntity;

        Task<T> GetAsync<T>(string tableName, string partitionKey, string rowKey) where T : TableEntity;

        Task<T> SaveOrReplaceAsync<T>(string tableName, T data) where T : TableEntity;

        Task<List<T>> WhereAsync<T>(string tableName, TableQuery<T> query) where T : TableEntity, new();
    }
}