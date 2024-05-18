using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public sealed record GenerateMapRequest
    {
        public int Size { get; init; }
        public int BoosterCount { get; init; }
        public int ObstacleCount { get; init; }
        public int SpiceCount { get; init; }
    }
}
