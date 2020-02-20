using System;

namespace Data.Models.Mapping.Responses
{
    public interface IBaseResponse<TKey>
    {
        TKey Id { get; set; }
    }

    public abstract class BaseResponse<TKey> : IBaseResponse<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }

    public class BaseResponse : BaseResponse<string>
    {

    }
}
