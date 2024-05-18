using GECA.Client.Console.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Domain.Entities
{
    public class Caterpillar
    {
        public int Id { get; set; }
        public int MaxSegments { get; set; } // Maximum number of segments the caterpillar can have

        public List<Segment> Segments { get; set; } = new List<Segment>();

        public Caterpillar()
        {
            // Initialize the caterpillar with head and tail segments
            Segments.Add(new Segment(SegmentType.Head));
            Segments.Add(new Segment(SegmentType.Tail));
        }
    }
}
