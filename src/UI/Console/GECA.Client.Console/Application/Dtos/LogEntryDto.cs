using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public record LogEntryDto : Dto
    {
        public DateTime Timestamp { get; set; }
        public string? Level { get; set; }
        public string? MessageTemplate { get; set; }
        public LogEntryPropertiesDto? Properties { get; set; }


    }
}
