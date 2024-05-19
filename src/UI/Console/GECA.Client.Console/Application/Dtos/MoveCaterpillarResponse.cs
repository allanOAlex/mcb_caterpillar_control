using GECA.Client.Console.Domain.Enums;
using System.ComponentModel;

namespace GECA.Client.Console.Application.Dtos
{
    public record MoveCaterpillarResponse : Response
    {

        [DefaultValue(false)]
        public bool FoundSpice{ get; init; }
        public int SpiceRow{ get; init; }
        public bool SpiceColumn{ get; init; }

        [DefaultValue(false)]
        public bool HitsObstacle { get; init; }
        public int ObstacleRow { get; init; }
        public bool ObstacleColumn { get; init; }

        [DefaultValue(false)]
        public bool EncounteredBooster { get; init; }
        public int BoosterRow { get; init; }
        public bool BoosterColumn { get; init; }

        [DefaultValue(false)]
        public bool CrossedBoundary { get; init; }
        public int BoundaryRow { get; init; }
        public bool BoundaryColumn { get; init; }

        public EventType EventType { get; init; }
        public int NewCatapillarRow { get; init; }
        public int NewCatapillarColumn { get; init; }
    }
}
