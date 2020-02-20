using System;
using AutoMapper;
using Core.Extensions;
using Data.Models.Identity;
using Data.Models.Mapping.Responses;

namespace Data.Models.Mapping.Resolvers
{
    /// <inheritdoc />
    /// <summary>
    /// Please check this link to understand using
    /// https://github.com/AutoMapper/AutoMapper/wiki/Custom-value-resolvers
    /// </summary>
    public class UserImageUrlResolver : IValueResolver<User, UserSimpleResponse, string>
    {
        private readonly ProjectConfiguration _configuration;
        public UserImageUrlResolver(ProjectConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string Resolve(User source, UserSimpleResponse destination, string destMember, ResolutionContext context)
        {
            return string.IsNullOrEmpty(source.ImageUrl)
                ? null
                : new Uri(new Uri(_configuration.StorageBaseUrl), source.ImageUrl).AbsoluteUri;
        }
    }
}
