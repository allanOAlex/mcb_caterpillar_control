using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Helpers
{
    internal static class LoggingHelper
    {
        public static List<LogEvent> DeserializeLogEvents(string[] logFileContents)
        {
            try
            {
                var logs = new List<LogEvent>();

                foreach (var line in logFileContents)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var logEvent = JsonSerializer.Deserialize<LogEvent>(line);
                        logs.Add(logEvent!);
                    }
                }

                return logs;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
