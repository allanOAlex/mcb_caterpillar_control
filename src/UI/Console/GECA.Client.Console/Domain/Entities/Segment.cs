using GECA.Client.Console.Domain.Enums;

namespace GECA.Client.Console.Domain.Entities
{
    public class Segment
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public SegmentType Type { get; } // Head, Tail, or Intermediate

        public Segment(SegmentType type)
        {
            Type = type;
        }
    }
}
