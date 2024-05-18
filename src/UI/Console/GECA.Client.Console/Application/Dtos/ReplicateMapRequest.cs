using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public record ReplicateMapRequest
    {
        public char[,] Map { get; init; }
        public int CaterpillarRow { get; set; }
        public int CaterpillarColumn { get; set; }
        public bool IsHorizontalMirroring { get; init; }
    }
}
