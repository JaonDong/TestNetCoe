using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using EventBus.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserNotifyService;
using UserNotifyService.Events;
using UserNotifyService.Services;

namespace TimeOutOrderService.Services
{
    public class ManagerService:IManagerService
    {
        private readonly ManagerSettings _settings;
        private readonly IEventBus _eventBus;
        private readonly ILogger<ManagerService> _logger;

        public ManagerService(IOptions<ManagerSettings> settings,
            IEventBus eventBus,
            ILogger<ManagerService> logger)
        {
            _settings = settings.Value;
            _eventBus = eventBus;
            _logger = logger;
        }

        public void CheckTimeoutCancelOrders()
        {
            var orderIds = GetTimeOutCancelOrders();

            foreach (var orderId in orderIds)
            {
                var timeoutCancelOrderEvent = new TimeoutCancelOrderIntegrationEvent(orderId);
                _eventBus.Publish(timeoutCancelOrderEvent);
            }
        }


        private IEnumerable<int> GetTimeOutCancelOrders()
        {
            IEnumerable<int> orderIds = new List<int>();
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                try
                {
                    conn.Open();
                    orderIds = conn.Query<int>(
                        @"SELECT Id FROM [OrderingDb].[ordering].[orders] 
                            WHERE DATEDIFF(minute, [OrderDate], GETDATE()) >= @GracePeriodTime
                            AND [OrderStatusId] = 1",
                        new { GracePeriodTime = _settings.GracePeriodTime });
                }
                catch (SqlException exception)
                {
                    _logger.LogCritical($"FATAL ERROR: Database connections could not be opened: {exception.Message}");
                }

            }

            return orderIds;
        }
    }
}