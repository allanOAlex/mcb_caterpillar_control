using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;

namespace GECA.Client.Console.Application.Dtos
{
    public record SegmentDto
    {
        public SegmentType Type { get; init; }

        public Segment Segment { get; init; }
        
    }
}
