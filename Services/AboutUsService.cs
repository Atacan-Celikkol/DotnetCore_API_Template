using Data;
using Data.Models.Common;

namespace Services
{
    public interface IAboutUsService : IDataService<AboutUs>
    {

    }
    public class AboutUsService : DataService<AboutUs>, IAboutUsService
    {
        public AboutUsService(DataContext context) : base(context)
        {
        }
    }
}
