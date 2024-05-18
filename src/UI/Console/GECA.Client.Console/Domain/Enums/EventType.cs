using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Domain.Enums
{
    public enum EventType
    {
        Moved,
        Obstacle,
        Booster,
        Spice,
        HorizontalCrossBoundary,
        VerticalCrossBoundary,
        CrossBoundary,
        HitMapBoundary
    }
}
