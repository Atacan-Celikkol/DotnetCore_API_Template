using Data.Models.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Presentation.Api.Token
{
    /// <summary>
    /// 
    /// </summary>
    public class JwtHelper
    {
        private readonly TokenProviderOptions _options;
        private readonly UserManager<User> _userManager;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="um"></param>
        public JwtHelper(TokenProviderOptions options, UserManager<User> um)
        {
            _options = options;
            _userManager = um;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="expireIn"></param>
        /// <returns></returns>
        public async Task<string> GenerateJwtTokenAsync(User user, TimeSpan? expireIn = null)
        {

            var roles = await _userManager.GetRolesAsync(user);
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, await _options.NonceGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim("UserId", user.Id)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expireIn ?? _options.Expiration),
                signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            //var protector = context.RequestServices.GetService<IDataProtectionProvider>();
            //encodedJwt = protector.CreateProtector(_options.Issuer).Protect(encodedJwt);
            return encodedJwt;
        }
    }
}
