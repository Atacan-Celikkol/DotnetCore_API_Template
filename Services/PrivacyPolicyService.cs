using Data;
using Data.Models.Common;

namespace Services
{
    public interface IPrivacyPolicyService : IDataService<PrivacyPolicy>
    {

    }
    public class PrivacyPolicyService : DataService<PrivacyPolicy>, IPrivacyPolicyService
    {
        public PrivacyPolicyService(DataContext context) : base(context)
        {
        }
    }
}
