using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Solu.Framework;
using Solu.Framework.Shared;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Solu.Shared
{
    public class NotificationHandler : INotificationHandler
    {
        private readonly IOptionsMonitor<ApplicationOptions> _options;

        private readonly IBusControl _busControl;

        public NotificationHandler(IOptionsMonitor<ApplicationOptions> options)
        {
            _busControl = Bus.Factory.CreateUsingRabbitMq();
            _options = options;
        }

        public async Task SendVerificationCodeAsync(string mobileNumber, string code, CancellationToken cancellationToken)
        {
            await _busControl.Publish<SMA.Service.Notification.Message.SMS>(new SMA.Service.Notification.Message.SMS()
            {
                MobileNumbers = new[] { mobileNumber },
                MessageId = _options.CurrentValue.NotificationMessageId,
                Parameters = new[] {
                    new SMA.Service.Notification.Message.Parameter()
                    {
                        Name = _options.CurrentValue.NotificationParameterName,
                        Value = code
                    }
                }.ToList(),
                ProfileId = _options.CurrentValue.NotificationProfileId
            });
        }
    }
}
