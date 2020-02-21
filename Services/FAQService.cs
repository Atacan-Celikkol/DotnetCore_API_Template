using Data;
using Data.Models.Common;

namespace Services
{
    public interface IFAQService : IDataService<FAQ>
    {
    }

    public class FAQService : DataService<FAQ>, IFAQService
    {
        public FAQService(DataContext context) : base(context)
        {
        }
    }
}