using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models.Mapping.Responses
{
    public class RoleResponse : BaseResponse
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}
