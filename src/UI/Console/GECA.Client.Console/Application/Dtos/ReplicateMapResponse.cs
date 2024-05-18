using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public record ReplicateMapResponse
    {
        public char[,] Map { get; init; }
        public int NewCaterpillarRow { get; init; }
        public int NewCaterpillarColumn { get; init; }
    }
}
