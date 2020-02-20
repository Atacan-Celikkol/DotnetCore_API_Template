using AutoMapper;

namespace Data.Models.Mapping.Resolvers
{
    public class EmptyStringConverter : IValueConverter<string, string>
    {
        public string Convert(string source, ResolutionContext context)
        {
            return string.IsNullOrWhiteSpace(source) ? null : source;
        }
    }
}