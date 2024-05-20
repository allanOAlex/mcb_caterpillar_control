using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Application.Abstractions.Intefaces;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.BaseCommands
{
    public abstract class GenericBaseMovementCommand : ICommandGeneric
    {
        protected readonly char[,] map;
        protected readonly int previousRow;
        protected readonly int previousColumn;
        protected readonly List<Segment> previousSegments;
        protected List<Segment> newSegments;
        protected readonly CaterpillarSimulation simulation;
        protected EventType eventType;
        protected readonly IServiceManager serviceManager;

        public GenericBaseMovementCommand(CaterpillarSimulation simulation, IServiceManager ServiceManager)
        {
            this.simulation = simulation;
            serviceManager = ServiceManager;
            map = simulation.map;
            previousRow = CaterpillarSimulation.caterpillarRow;
            previousColumn = CaterpillarSimulation.caterpillarColumn;
            previousSegments = new List<Segment>(simulation.Caterpillar.Segments);
        }

        public abstract void Execute();

        public virtual void Undo()
        {
            // Restore caterpillar position
            CaterpillarSimulation.caterpillarRow = previousRow;
            CaterpillarSimulation.caterpillarColumn = previousColumn;
            map[CaterpillarSimulation.caterpillarRow, CaterpillarSimulation.caterpillarColumn] = 'C';

            // Restore caterpillar segments
            simulation.Caterpillar.Segments = new List<Segment>(previousSegments);

            // Handle specific event type reversion
            switch (eventType)
            {
                case EventType.Obstacle:
                    serviceManager.CaterpillarService.UnDestroyCaterpillar(map, previousRow, previousColumn);
                    break;
                case EventType.Booster:
                    // Revert the caterpillar size change
                    if (previousSegments.Count > newSegments.Count)
                    {
                        simulation.Caterpillar.Segments.RemoveAt(simulation.Caterpillar.Segments.Count - 1);
                    }
                    else if (previousSegments.Count < newSegments.Count)
                    {
                        simulation.Caterpillar.Segments.Add(new Segment(SegmentType.Intermediate));
                    }
                    map[previousRow, previousColumn] = 'B';
                    break;
                case EventType.Spice:
                    map[previousRow, previousColumn] = 'S';
                    break;
            }
        }
    }
}
