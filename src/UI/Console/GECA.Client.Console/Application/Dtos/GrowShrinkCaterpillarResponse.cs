using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public record GrowShrinkCaterpillarResponse : Response
    {
        public bool CaterpillarGrown { get; set; }
        public bool CaterpillarShrunk { get; set; }
        public int InitialSegments { get; set; }
        public int CurrentSegments { get; set; }
    }
}
