using System.Net;

namespace Core.Exceptions
{
    public class ResourceUnauthorizedException : BaseException
    {
        public ResourceUnauthorizedException() : base("User doesn't have any permission to do this operation.")
        {
            StatusCode = HttpStatusCode.Forbidden;
        }

        public ResourceUnauthorizedException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.Forbidden;
        }
    }
}
