using EventBus.Events;

namespace Order.Api.Events
{
    public class TimeoutCancelOrderIntegrationEvent : IntegrationEvent
    {
        public int OrderId { get; }

        public TimeoutCancelOrderIntegrationEvent(int orderId)
        {
            OrderId = orderId;
        }
    }
}