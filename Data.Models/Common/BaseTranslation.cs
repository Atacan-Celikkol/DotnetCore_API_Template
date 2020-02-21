using System.ComponentModel.DataAnnotations;

namespace Data.Models.Common
{
    public interface ILanguageCode
    {
        string LanguageCode { get; set; }
    }

    public interface ITranslation : ILanguageCode
    {
        string Translation { get; set; }
    }

    public class BaseTranslation : BaseEntity, ILanguageCode
    {
        [MaxLength(10)]
        [Required]
        public string LanguageCode { get; set; }
    }
}