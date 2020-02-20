using AutoMapper;
using Core.Extensions;

namespace Data.Models.Mapping.Resolvers
{
    public class ShareUrlValueConverter : IValueConverter<string, string>
    {
        private readonly ProjectConfiguration projectConfiguration;
        public ShareUrlValueConverter(ProjectConfiguration pc)
        {
            projectConfiguration = pc;
        }
        
        public string Convert(string path, ResolutionContext context)
        {
            return $"{projectConfiguration.WebsiteBaseUrl}/{path}";
        }
    }
}
