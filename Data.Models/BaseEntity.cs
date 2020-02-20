using System;
using System.ComponentModel.DataAnnotations;
using Core.Extensions;


namespace Data.Models
{
    public interface IBaseEntity : IBaseEntity<string>
    {

    }
    public interface IBaseEntity<TKey>
    {
        TKey Id { get; set; }
        bool IsDeleted { get; set; }
        DateTimeOffset CreateDate { get; set; }
        DateTimeOffset? DeleteDate { get; set; }
    }

    public abstract class BaseEntity<TKey> : IBaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        [Key]
        public TKey Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset? DeleteDate { get; set; }
    }

    public class BaseEntity : BaseEntity<string>, IBaseEntity
    {
        public BaseEntity()
        {
            Id = SequentialGuid.NewGuid().ToString();
            CreateDate = DateTimeOffset.UtcNow;
        }
    }
}
