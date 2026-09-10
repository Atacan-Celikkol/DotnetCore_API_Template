using Slack.Webhooks;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Providers.Sms
{
    public class SlackSmsProvider : ISmsProvider
    {
        private readonly SlackClient _slackClient;

        public SlackSmsProvider()
        {
            _slackClient = new SlackClient("");
        }

        public async Task SendAsync(string to, string @from, string message)
        {
            var slackMessage = new SlackMessage
            {
                Channel = "#{PROJECTNAME}-notifications",
                Text = $"{to} - {message}",
                IconEmoji = Emoji.Phone,
                Username = "{PROJECTNAME}",
                Markdown = true,
            };
            // ReSharper disable once UnusedVariable
            var response = await _slackClient.PostAsync(slackMessage);
        }

        public async Task SendAsync(IEnumerable<string> to, string @from, string message)
        {
            foreach (var number in to)
            {
                await SendAsync(number, @from, message);
            }
        }
    }
}