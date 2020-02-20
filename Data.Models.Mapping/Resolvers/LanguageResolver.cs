using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AutoMapper;
using Data.Models.Common;

namespace Data.Models.Mapping.Helpers
{
    public static class LanguageExtensions
    {
        public static T GetCurrentTranslation<T>(this ICollection<T> translations, string languageCode) where T : BaseTranslation
        {
            foreach (var translation in translations)
            {
                if (translation is BaseTranslation baseTranslation && string.Equals(baseTranslation.LanguageCode, languageCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    return translation;
                }
            }
            return default;
        }

        public static T GetCurrentTranslation<T>(this ICollection<T> translations) where T : BaseTranslation
        {
            return GetCurrentTranslation(translations, Thread.CurrentThread.CurrentCulture.Name.ToLowerInvariant());
        }

        public static T GetCurrentTranslation<T>(this ICollection<T> translations, ResolutionContext context) where T : BaseTranslation
        {
            var languageCode = context.Items.Keys.Contains("LanguageCode")
                ? context.Items["LanguageCode"].ToString()
                : Thread.CurrentThread.CurrentCulture.Name.ToLowerInvariant();
            return GetCurrentTranslation(translations, languageCode);
        }

        public static string GetTranslation<T>(this ICollection<T> translations, string propertyName, ResolutionContext context) where T : BaseTranslation
        {
            var languageCode = context.Items.Keys.Contains("LanguageCode")
                ? context.Items["LanguageCode"].ToString()
                : Thread.CurrentThread.CurrentCulture.Name.ToLowerInvariant();
            return GetTranslation(translations, propertyName, languageCode);
        }

        public static string GetTranslation<T>(this ICollection<T> translations, string propertyName) where T : BaseTranslation
        {
            return GetTranslation(translations, propertyName, Thread.CurrentThread.CurrentCulture.Name.ToLowerInvariant());
        }

        public static string GetTranslation<T>(this ICollection<T> translations, string propertyName,
            string languageCode) where T : BaseTranslation
        {


            var translation = translations.FirstOrDefault(t =>
                string.Equals(t.LanguageCode, languageCode, StringComparison.InvariantCultureIgnoreCase));
            if (translation == null)
            {
                return string.Empty;
            }

            var property = translation.GetType().GetProperty(propertyName);

            if (property == null)
            {
                return string.Empty;
            }
            try
            {
                return (string)property.GetValue(translation);
            }
            catch
            {
                var array = (string[])property.GetValue(translation);
                return string.Join(',', array);
            }
        }
    }
}
