using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public record Response
    {
        public bool Successful { get; init; }
        public string Message { get; init; }
    }
}
