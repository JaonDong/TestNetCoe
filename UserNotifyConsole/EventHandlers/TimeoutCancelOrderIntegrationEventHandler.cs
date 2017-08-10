using System;
using System.Threading;
using System.Threading.Tasks;
using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Order.Api.Events;

namespace UserNotifyConsole.EventHandlers
{
    public class TimeoutCancelOrderIntegrationEventHandler: IIntegrationEventHandler<TimeoutCancelOrderIntegrationEvent>
    {
        private readonly ILogger<TimeoutCancelOrderIntegrationEventHandler> _logger;
        public TimeoutCancelOrderIntegrationEventHandler(ILogger<TimeoutCancelOrderIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TimeoutCancelOrderIntegrationEvent @event)
        {
            _logger.LogInformation($"UserNotifyConsole received sub event：" + @event.Id);
            return Task.CompletedTask;
        }
    }
}