using System;

namespace Data.Models.Mapping.Responses.Common
{
    public class FAQResponse : BaseResponse
    {
        public int Version { get; set; }
        public string Details { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
