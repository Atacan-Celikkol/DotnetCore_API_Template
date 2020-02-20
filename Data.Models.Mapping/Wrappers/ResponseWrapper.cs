using Core.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Data.Models.Mapping.Wrappers
{
    public class Response
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ApiResponseStatus Status { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string StackTrace { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ErrorCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Error> Errors { get; set; }

        public override string ToString()
        {
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(this, jsonSerializerSettings);
        }

        public static Response Create(BaseException exception)
        {
            return new Response()
            {
                Status = ApiResponseStatus.Error,
                ErrorCode = exception.ErrorCode,
                Message = exception.Message,
            };
        }

        public static Response Create()
        {
            return new Response()
            {
                Status = ApiResponseStatus.Ok
            };
        }
    }

    public class Response<T> : Response
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }

        public static Response<T> Create(T data)
        {
            return new Response<T>()
            {
                Data = data,
                Status = ApiResponseStatus.Ok
            };
        }
    }

    public class PaginatedResponse<T> : Response<List<T>>
    {
        public int TotalItemCount { get; set; }
        public int CurrentPage { get; set; }
        public int Size { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? NextPage { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? PreviousPage { get; set; }

        public static PaginatedResponse<T> Create(List<T> data, int totalItemCount, int currentPage, int size, int? nextPage, int? previous)
        {
            return new PaginatedResponse<T>()
            {
                Data = data,
                Status = ApiResponseStatus.Ok,
                TotalItemCount = totalItemCount,
                Size = size,
                CurrentPage = currentPage,
                NextPage = nextPage,
                PreviousPage = previous
            };
        }
    }

    public enum ApiResponseStatus
    {
        Error = -1,
        Ok = 1,
    }

    public class Error
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public int Code { get; set; }
    }
}
