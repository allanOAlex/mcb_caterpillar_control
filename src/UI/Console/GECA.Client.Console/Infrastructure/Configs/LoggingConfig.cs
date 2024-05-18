using Serilog.Core.Enrichers;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GECA.Client.Console.Infrastructure.Extensions.DependencyInjection;

namespace GECA.Client.Console.Infrastructure.Configs
{
    public class LoggingConfig
    {
        public string LogFile { get; set; } = "caterpillar_control_log.txt"; // Path to log file
        public LogEventLevel MinimumLevel { get; set; } // Minimum log level for all loggers
        public Dictionary<string, LogEventLevel> OverrideLevels { get; set; } // Override levels for specific loggers
        public IEnumerable<ILogEventEnricher> Enrichers { get; set; } // List of enrichers to add properties to logs
        public Func<LogEvent, bool> Filter { get; set; } // Filter function to exclude specific logs

        public static LoggingConfig CreateDefaultConfig()
        {
            return new LoggingConfig
            {
                LogFile = "caterpillar_control_log.txt",
                MinimumLevel = LogEventLevel.Information,
                OverrideLevels = new Dictionary<string, LogEventLevel>
                {
                    ["System"] = LogEventLevel.Warning,
                    ["Serilog"] = LogEventLevel.Warning,
                    ["Microsoft"] = LogEventLevel.Warning
                },
                Enrichers = new List<ILogEventEnricher>
                {
                    new CorrelationIdEnricher(),
                    new IPAddressEnricher(),
                    new PropertyEnricher("MachineName", Environment.MachineName) // Assuming PropertyEnricher exists with similar functionality
                },
                Filter = logEvent =>
                {
                    if (logEvent.Properties.TryGetValue("SourceContext", out var value) &&
                        value is ScalarValue scalarValue &&
                        scalarValue.Value is string sourceContext)
                    {
                        return !sourceContext.StartsWith("Microsoft.");
                    }
                    return false;
                }
            };
        }
    }
}
