using System;

namespace Data.Models.Mapping.Responses
{
    public class AboutUsResponse : BaseResponse
    {
        public int Version { get; set; }
        public string Details { get; set; }
        public DateTimeOffset CreateDate { get; set; }
    }
}
