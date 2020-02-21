using AutoMapper;
using Core.Extensions;
using System;

namespace Data.Models.Mapping.Resolvers
{
    public class ImageValueConverter : IValueConverter<string, string>
    {
        private readonly ProjectConfiguration projectConfiguration;

        public ImageValueConverter(ProjectConfiguration pc)
        {
            projectConfiguration = pc;
        }

        public string Convert(string sourceMember, ResolutionContext context)
        {
            return string.IsNullOrEmpty(sourceMember)
               ? null
               : new Uri(new Uri(projectConfiguration.StorageBaseUrl), sourceMember).AbsoluteUri;
        }
    }
}