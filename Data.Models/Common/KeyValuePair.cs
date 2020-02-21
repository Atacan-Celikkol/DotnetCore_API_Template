using System.ComponentModel.DataAnnotations;

namespace Data.Models.Common
{
    public class KeyValuePair : BaseEntity
    {
        [MaxLength(50)]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}