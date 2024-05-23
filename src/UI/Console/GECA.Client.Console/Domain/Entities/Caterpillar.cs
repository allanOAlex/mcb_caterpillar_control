using GECA.Client.Console.Domain.Enums;

namespace GECA.Client.Console.Domain.Entities
{
    public class Caterpillar
    {
        public int Id { get; set; }
        public int PreviousRow { get; set; }
        public int PreviousColumn { get; set; }
        public int CurrentRow { get; set; }
        public int CurrentColumn { get; set; }
        public int MaxSegments { get; set; } 
        public int CurrentSegmentCount { get; set; } 
        public int PreviousSegmentCount { get; set; }

        public List<Segment> PreviousSegments { get; set; }
        public List<Segment> Segments { get; set; } = new();

        public Caterpillar()
        {
            // Initialize the caterpillar with head and tail segments
            Segments.Add(new Segment(SegmentType.Head));
            Segments.Add(new Segment(SegmentType.Tail));
        }
    }
}
