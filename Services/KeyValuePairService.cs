using System;
using System.Globalization;
using System.Threading.Tasks;
using Data;
using Data.Models.Common;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public interface IKeyValuePairService : IDataService<KeyValuePair>
    {
        Task<T> GetAsync<T>(string key);
        Task<KeyValuePair> GetResourceAsync(string key);
        Task UpsertAsync(string key, string value);
    }

    public class KeyValuePairService : DataService<KeyValuePair>, IKeyValuePairService
    {
        public KeyValuePairService(DataContext context) : base(context)
        {
        }

        public async Task<T> GetAsync<T>(string key)
        {
            key = key.ToLowerInvariant();
            var resource = await Context.KeyValuePairs.SingleOrDefaultAsync(t => t.Key == key);
            if (resource != null)
            {
                return (T)Convert.ChangeType(resource.Value, typeof(T), CultureInfo.InvariantCulture.NumberFormat);
            }
            return default(T);
        }

        public async Task<KeyValuePair> GetResourceAsync(string key)
        {
            key = key.ToLowerInvariant();
            return await Context.KeyValuePairs.SingleOrDefaultAsync(t => t.Key == key);
        }

        public async Task UpsertAsync(string key, string value)
        {
            key = key.ToLowerInvariant();
            var resource = await GetResourceAsync(key);
            if (resource == null)
            {
                resource = new KeyValuePair
                {
                    Key = key,
                    Value = value
                };
                Context.KeyValuePairs.Add(resource);
            }
            else
            {
                resource.Value = value;
            }
            await Context.SaveChangesAsync();
        }
    }
}
