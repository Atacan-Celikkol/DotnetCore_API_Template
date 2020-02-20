using System;

namespace Data.Models
{
    public interface IAuditedEntity : IAuditedEntity<string>
    {

    }

    public interface IAuditedEntity<TKey> : IBaseEntity<TKey>
    {
        DateTimeOffset? LastModifiedDate { get; set; }
        string ModifiedBy { get; set; }
    }
    public class AuditedEntity<TKey> : BaseEntity<TKey>, IAuditedEntity<TKey> where TKey : IEquatable<TKey>
    {
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class AuditedEntity : BaseEntity, IAuditedEntity
    {
        public DateTimeOffset? LastModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }

}
