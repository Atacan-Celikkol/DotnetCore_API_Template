using Microsoft.AspNetCore.SignalR;

namespace Presentation.Api.Providers
{
    /// <summary>
    ///
    /// </summary>
    public class UserIdProvider : IUserIdProvider
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public virtual string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst("UserId")?.Value;
        }
    }
}