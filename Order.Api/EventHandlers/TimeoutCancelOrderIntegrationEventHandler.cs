﻿using System.Threading.Tasks;
using EventBus.Abstractions;
using Order.Api.Events;

namespace Order.Api.EventHandlers
{
    public class TimeoutCancelOrderIntegrationEventHandler: IIntegrationEventHandler<TimeoutCancelOrderIntegrationEvent>
    {
        public Task Handle(TimeoutCancelOrderIntegrationEvent @event)
        {
           return  Task.Run(() =>
           {
               //TODO :do  something
           });
        }
    }
}