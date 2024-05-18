using GECA.Client.Console.Domain.Entities;

namespace GECA.Client.Console.Application.Dtos
{
    public record CaterpillarDto
    {
        public List<Segment> Segments { get; } = new List<Segment>();

        public Caterpillar Caterpillar { get; init; }
        
    }
}
