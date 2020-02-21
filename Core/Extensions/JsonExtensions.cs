using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Extensions
{
    public static class JsonExtensions
    {
        public static void PopulateObject<T>(this JToken jToken, T target)
        {
            var serializer = new JsonSerializer();
            serializer.Populate(jToken.CreateReader(), target);
        }
    }
}