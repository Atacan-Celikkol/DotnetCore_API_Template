using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;


namespace Presentation.Api.Token
{
    /// <summary>
    /// 
    /// </summary>
    public static class MiddlewareExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTokenProvider(
            this IApplicationBuilder builder, TokenProviderOptions parameters)
        {
            return builder.UseMiddleware<TokenProviderMiddleware>(Options.Create(parameters));
        }
    }
}
