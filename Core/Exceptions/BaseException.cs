using System;
using System.Collections.Generic;
using System.Net;

namespace Core.Exceptions
{
    public class BaseException : Exception
    {
        public int ErrorCode { get; set; }
        public string Key { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public List<Dictionary<string, string>> Extras { get; set; }

        public BaseException() : base("default exception")
        {
            Extras = new List<Dictionary<string, string>>();
            Key = GetType().Name;
            ErrorCode = UniqueHash(GetType().Name);
            StatusCode = HttpStatusCode.BadRequest;
        }

        public BaseException(string message) : base(message)
        {
            Extras = new List<Dictionary<string, string>>();
            Key = GetType().Name;
            ErrorCode = UniqueHash(GetType().Name);
            StatusCode = HttpStatusCode.BadRequest;
        }

        public BaseException(string message, HttpStatusCode code) : base(message)
        {
            Extras = new List<Dictionary<string, string>>();
            Key = GetType().Name;
            ErrorCode = UniqueHash(GetType().Name);
            StatusCode = code;
        }      
        private static int UniqueHash(string value)
        {
            int h = 0;
            for (int i = 0; i < value.Length; i++)
                h += value[i] * 31 ^ value.Length - (i + 1);
            return h;
        }
    }
}
