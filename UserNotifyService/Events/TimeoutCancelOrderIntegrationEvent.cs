using EventBus.Events;

namespace UserNotifyService.Events
{
    public class TimeoutCancelOrderIntegrationEvent:IntegrationEvent
    {
        public int OrderId { get; }

        public TimeoutCancelOrderIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
    }
}