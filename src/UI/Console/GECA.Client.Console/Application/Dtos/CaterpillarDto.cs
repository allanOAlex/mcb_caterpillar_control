using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public record CaterpillarDto
    {
        public List<Segment> Segments { get; } = new List<Segment>();

        public Caterpillar Caterpillar { get; init; }
        
    }
}
