using System.Net;

namespace Core.Exceptions
{
    public class UserUnauthorizedException : BaseException
    {
        public UserUnauthorizedException() : base("User is unauthorized!")
        {
            StatusCode = HttpStatusCode.Unauthorized;
        }
    }
}