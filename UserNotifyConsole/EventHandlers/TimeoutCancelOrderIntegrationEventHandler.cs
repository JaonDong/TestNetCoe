using System;
using System.Threading.Tasks;
using EventBus.Abstractions;
using Order.Api.Events;

namespace UserNotifyConsole.EventHandlers
{
    public class TimeoutCancelOrderIntegrationEventHandler: IIntegrationEventHandler<TimeoutCancelOrderIntegrationEvent>
    {
        public Task Handle(TimeoutCancelOrderIntegrationEvent @event)
        {

           return  Task.Run(() =>
            {
                Console.WriteLine(@event.OrderId);
            });
        }
    }
}