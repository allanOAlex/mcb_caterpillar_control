using GECA.Client.Console.Domain.Enums;
using System.ComponentModel;

namespace GECA.Client.Console.Application.Dtos
{
    public record MoveCaterpillarResponse : Response
    {

        [DefaultValue(false)]
        public bool HitsObstacle { get; init; }

        [DefaultValue(false)]
        public bool EncounteredBooster { get; init; }

        [DefaultValue(false)]
        public bool CrossedBoundary { get; init; }

        public EventType EventType { get; init; }
        public int NewCatapillarRow { get; init; }
        public int NewCatapillarColumn { get; init; }
    }
}
