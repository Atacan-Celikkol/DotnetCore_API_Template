using Core.Extensions;
using Data.Models.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Presentation.Api.Token
{
    /// <summary>
    ///
    /// </summary>
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        private readonly JsonSerializerSettings _serializerSettings;

        /// <summary>
        ///
        /// </summary>
        /// <param name="next"></param>
        /// <param name="options"></param>
        public TokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options)
        {
            _next = next;

            _options = options.Value;
            ThrowIfInvalidOptions(_options);

            _serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
            };
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Invoke(HttpContext context)
        {
            //Path Control for token and otp-token path
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal) && !context.Request.Path.Equals(_options.OtpPath, StringComparison.Ordinal))
            {
                return _next(context);
            }

            //token generate
            if (context.Request.Method.Equals("POST") && context.Request.HasFormContentType && context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return GenerateToken(context);
            }

            //token generate with otp
            if (context.Request.Method.Equals("POST") && context.Request.HasFormContentType && _options.IsOtpTokenPathEnabled && context.Request.Path.Equals(_options.OtpPath, StringComparison.Ordinal))
            {
                return GenerateOtpToken(context);
            }
            context.Response.StatusCode = 400;
            return context.Response.WriteAsync("Bad request.");
        }

        private async Task GenerateOtpToken(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var otp = context.Request.Form["otp"];
            var manager = context.RequestServices.GetService<UserManager<User>>();
            var configuration = context.RequestServices.GetService<ProjectConfiguration>();
            var user = await manager.FindByNameAsync(username);
            if (user == null)
            {
                await GenereateErrorResponseAsync(context, "Hatalı kullanıcı adı ya da şifre");
                return;
            }
            var correctCode = user != null && await manager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider, otp);
            var testCode = !configuration.IsProduction && user != null && otp == "258369";
            if (user == null)
            {
            }
            if (!testCode && !correctCode)
            {
                user.LoginAttempts++;
                await manager.UpdateAsync(user);
                if (user.LoginAttempts > 2)
                {
                    await GenereateErrorResponseAsync(context, "2 kereden fazla hatalı kod giridiniz");
                    return;
                }

                await GenereateErrorResponseAsync(context, "Geçersiz Kod");
                return;
            }
            if (user.LoginAttempts > 2)
            {
                await GenereateErrorResponseAsync(context, "2 kereden fazla hatalı kod giridiniz");
                return;
            }

            if (user.LastCodeSendDate != null && (DateTimeOffset.UtcNow - user.LastCodeSendDate.Value).TotalMinutes > 2)
            {
                await GenereateErrorResponseAsync(context, "Süresi geçmiş kod. Lütfen tekrar kod isteyiniz.");
            }
            user.LoginAttempts = 0;
            await manager.UpdateAsync(user);
            await GenerateJwtAsync(context, user);
        }

        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];
            var manager = context.RequestServices.GetService<UserManager<User>>();
            var user = await manager.FindByNameAsync(username);
            var correctPassword = user != null && await manager.CheckPasswordAsync(user, password);
            if (!correctPassword)
            {
                await GenereateErrorResponseAsync(context, "Hatalı kullanıcı adı ya da şifre");
                return;
            }
            await GenerateJwtAsync(context, user);
        }

        private async Task GenerateJwtAsync(HttpContext context, User user)
        {
            var helper = context.RequestServices.GetService<JwtHelper>();
            var encodedJwt = await helper.GenerateJwtTokenAsync(user);
            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds
            };
            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, _serializerSettings));
        }

        private static void ThrowIfInvalidOptions(TokenProviderOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (string.IsNullOrEmpty(options.Path))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Path));
            }

            if (string.IsNullOrEmpty(options.Issuer))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Issuer));
            }

            if (string.IsNullOrEmpty(options.Audience))
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.Audience));
            }

            if (options.Expiration == TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(TokenProviderOptions.Expiration));
            }

            //if (options.IdentityResolver == null)
            //{
            //    throw new ArgumentNullException(nameof(TokenProviderOptions.IdentityResolver));
            //}

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.SigningCredentials));
            }

            if (options.NonceGenerator == null)
            {
                throw new ArgumentNullException(nameof(TokenProviderOptions.NonceGenerator));
            }
        }

        private async Task GenereateErrorResponseAsync(HttpContext context, string errorMessage)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new { Error = errorMessage }, _serializerSettings));
        }
    }
}