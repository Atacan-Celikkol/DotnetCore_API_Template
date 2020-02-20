using Core.Extensions;
using Infobip.Api.Client;
using Infobip.Api.Config;
using Infobip.Api.Model.Sms.Mt.Send.Textual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Providers.Sms
{
    public class InfobipSmsProvider : ISmsProvider
    {
        private readonly SendSingleTextualSms _client;
        public InfobipSmsProvider(string username, string password)
        {
            _client = new SendSingleTextualSms(new BasicAuthConfiguration(username, password));
        }

        public InfobipSmsProvider(string apiKey)
        {
            _client = new SendSingleTextualSms(new ApiKeyAuthConfiguration(apiKey));
        }
        public async Task SendAsync(string to, string from, string message)
        {
            to = to.Replace("+", "");
            var request = new SMSTextualRequest
            {
                From = from,
                To = new List<string>() { to },
                Text = message.ToNonTurkishString()
            };
            await _client.ExecuteAsync(request);
        }

        public async Task SendAsync(IEnumerable<string> to, string from, string message)
        {
            var request = new SMSTextualRequest
            {
                From = from,
                To = to.Select(t => t.Replace("+", "")).ToList(),
                Text = message.ToNonTurkishString()
            };
            await _client.ExecuteAsync(request);
        }
    }
}
