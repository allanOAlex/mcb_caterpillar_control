using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public record SegmentDto
    {
        public SegmentType Type { get; init; }

        public Segment Segment { get; init; }
        
    }
}
