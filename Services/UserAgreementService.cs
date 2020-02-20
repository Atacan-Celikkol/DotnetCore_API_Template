using Data;
using Data.Models.Common;

namespace Services
{
    public interface IUserAgreementService : IDataService<UserAgreement>
    {

    }
    public class UserAgreementService : DataService<UserAgreement>, IUserAgreementService
    {
        public UserAgreementService(DataContext context) : base(context)
        {
        }
    }
}
