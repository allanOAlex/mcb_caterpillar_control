using GECA.Client.Console.Domain.Entities;

namespace GECA.Client.Console.Shared
{
    public static class AppConstants
    {
        public static int CurrentCaterpillarRow { get; set; }
        public static int CurrentCaterpillarColumn { get; set; }
        public static int RadarRange { get; set; } = 11;
        public static List<Spice> SpiceList { get; set; }
        public static int CurrentSegmentCount { get; set; }
        public static int PreviousSegmentCount { get; set; }
        public static List<Segment> PreviousCaterpillarSegments { get; set; }
        public static string? Direction { get; set; }
        public static int Steps { get; set; }
        public static bool GrowOrShrink { get; set; }
    }
}
