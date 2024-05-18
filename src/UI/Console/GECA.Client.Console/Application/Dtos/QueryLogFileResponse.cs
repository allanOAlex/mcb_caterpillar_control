using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public record QueryLogFileResponse : Response
    {
        public string? TimeStamp { get; set; }
        public string? Level { get; set; }
        public string? MessageTemplate { get; set; }
        public string? Method { get; set; }
        public string? Time { get; set; }
        public string? RequestId { get; set; }
        public string? RequestPath { get; set; }
        public string? ConnectionId { get; set; }
        public string? CorrelationId { get; set; }
        public string? MachineName { get; set; }



    }
}
