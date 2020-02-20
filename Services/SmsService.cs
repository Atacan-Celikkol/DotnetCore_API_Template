using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data;
using Providers.Sms;

namespace Services
{
    public interface ISmsService : IDataService
    {
        Task SendOtpCodeAsync(string phoneNumber, string code);
    }

    public class SmsService : DataService, ISmsService
    {
        private readonly ISmsProvider _smsProvider;
        public SmsService(DataContext context, ISmsProvider smsProvider) : base(context)
        {
            _smsProvider = smsProvider;
        }

        public override bool IsAuthorizedForResource(string resourceId, string userId)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> IsAuthorizedForResourceAsync(string resourceId, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task SendOtpCodeAsync(string phoneNumber, string code)
        {
            await _smsProvider.SendAsync(phoneNumber, "{PROJECTNAME}", $"{code} ile PROJECTNAME giriş yapabilirsiniz.");
        }
    }
}
