using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Order.Api.Events;
using RabbitMQ.Client;
using TestNetCore.EventBusRabbitMQ;
using UserNotifyConsole.EventHandlers;

namespace UserNotifyConsole
{
    class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            StartUp();

            IServiceCollection services = new ServiceCollection();
            var serviceProvider = ConfigureServices(services);
            ConfigureEventBus(serviceProvider);

            var logger = serviceProvider.GetService<ILoggerFactory>();
            Configure(logger);

            Console.ReadKey();
        }

        public static void StartUp()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public static IServiceProvider ConfigureServices(IServiceCollection services)
        {
            #region 日志注入
            services.AddLogging()
                       .AddOptions(); 
            #endregion

            #region rabbit connection接口注入
            services.AddSingleton<IRabbitMQPersistentConnection>(sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<DefaultRabbitMQPersistentConnection>>();

                    var factory = new ConnectionFactory()
                    {
                        HostName = Configuration["EventBusConnection"]
                    };

                    return new DefaultRabbitMQPersistentConnection(factory, logger);
                }); 
            #endregion

            RegisterEventBus(services);

            #region autofac ico
            var container = new ContainerBuilder();
            container.Populate(services);
            return new AutofacServiceProvider(container.Build()); 
            #endregion
        }

        #region 日志配置
        public static void Configure(ILoggerFactory loggerFactory)
        {
            loggerFactory
                .AddConsole(Configuration.GetSection("Logging"))
                .AddConsole(LogLevel.Debug);
        }
        #endregion

        #region eventbus订阅
        private static void ConfigureEventBus(IServiceProvider serviceProvider)
        {
            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            eventBus.Subscribe<TimeoutCancelOrderIntegrationEvent, TimeoutCancelOrderIntegrationEventHandler>();
        }
        #endregion

        #region eventbus相关注入
        private static void RegisterEventBus(IServiceCollection services)
        {
            services.AddSingleton<IEventBus, EventBusRabbitMQ>();
            services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
            services.AddTransient<TimeoutCancelOrderIntegrationEventHandler>();

        } 
        #endregion
    }
}