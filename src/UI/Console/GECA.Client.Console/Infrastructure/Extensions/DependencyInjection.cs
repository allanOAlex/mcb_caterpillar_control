using GECA.Client.Console.Application.Abstractions.Intefaces;
using GECA.Client.Console.Application.Abstractions.IRepositories;
using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Infrastructure.Implementations.Interfaces;
using GECA.Client.Console.Infrastructure.Implementations.Repositories;
using GECA.Client.Console.Infrastructure.Implementations.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Formatting.Json;
using System.Net;

namespace GECA.Client.Console.Infrastructure.Extensions
{
    public static class DependencyInjection
    {
        private static IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();

        public class CorrelationIdEnricher : ILogEventEnricher
        {
            private const string CorrelationIdPropertyName = "CorrelationId";

            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                var correlationId = GetCorrelationId(); // Implement your logic to retrieve or generate a correlation ID
                var correlationIdProperty = new LogEventProperty(CorrelationIdPropertyName, new ScalarValue(correlationId));

                logEvent.AddOrUpdateProperty(correlationIdProperty);
            }

            private string GetCorrelationId()
            {
                // Retrieve the correlation ID from the HTTP context if available
                var httpContext = httpContextAccessor.HttpContext;
                var correlationId = httpContext?.Request.Headers["CorrelationId"].FirstOrDefault();

                // If the correlation ID is not available in the HTTP context, generate a new GUID
                if (string.IsNullOrEmpty(correlationId))
                {
                    correlationId = Guid.NewGuid().ToString();
                }

                return correlationId;
            }
        }

        public class IPAddressEnricher : ILogEventEnricher
        {
            public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
            {
                var httpContext = httpContextAccessor.HttpContext;
                var remoteIpAddress = httpContext?.Connection?.RemoteIpAddress;

                // Check if the IP address is available and not in IPv6 loopback format
                if (remoteIpAddress != null && !IPAddress.IsLoopback(remoteIpAddress))
                {
                    var ipAddress = remoteIpAddress.ToString();
                    var ipAddressProperty = propertyFactory.CreateProperty("IPAddress", ipAddress);
                    logEvent.AddPropertyIfAbsent(ipAddressProperty);
                }
            }
        }

        private static string GetLogFilePath(IConfiguration configuration)
        {
            string logFilePath = configuration["Logging:Serilog:LogFile"]!;
            string logFileName = $"{DateTime.Now:dd-MM-yyyy HH:mm:ss}.txt";
            var filePath = Path.Combine(logFilePath, logFileName);
            return filePath;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                //using var serviceProvider = services.BuildServiceProvider();
                //var configuration = serviceProvider.GetRequiredService<IConfiguration>();

                services.AddScoped<IServiceManager, ServiceManager>();
                services.AddScoped<ICaterpillarService, CaterpillarService>();
                services.AddScoped<IMapService, MapService>();
               
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
                services.AddScoped<ICaterpillarRepository, CaterpillarRepository>();
                services.AddScoped<ICaterpillarRepository, InMemoryCaterpillarRepository>();
                services.AddScoped<ISpiceRepository, InMemorySpiceRepository>();


                Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Serilog", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.With(new CorrelationIdEnricher())
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                .Enrich.With<IPAddressEnricher>()
                .Filter.ByExcluding(logEvent =>
                {
                    if (logEvent.Properties.TryGetValue("SourceContext", out var value) &&
                        value is ScalarValue scalarValue &&
                        scalarValue.Value is string sourceContext)
                    {
                        return sourceContext.StartsWith("Microsoft.");
                    }

                    return false;
                })
                .WriteTo.Async(s => s.File(new JsonFormatter(), configuration["Logging:Serilog:LogFile"]!, rollingInterval: RollingInterval.Day))
                .CreateLogger();

                services.AddSingleton<ILoggerFactory>(provider =>
                {
                    return new SerilogLoggerFactory(Log.Logger, true);
                });

                services.AddMemoryCache();

                return services;
            }
            catch (Exception)
            {

                throw;
            }



        }


    }
}
