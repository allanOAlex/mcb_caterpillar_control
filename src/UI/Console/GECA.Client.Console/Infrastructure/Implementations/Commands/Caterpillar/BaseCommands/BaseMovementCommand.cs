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
    public abstract class BaseMovementCommand : ICommand
    {
        //protected readonly char[,] map;
        //protected readonly int caterpillarRow;
        //protected readonly int caterpillarColumn;
        //protected readonly CaterpillarSimulation simulation;

        protected readonly CaterpillarSimulation simulation;
        protected readonly IServiceManager serviceManager;



        protected char[,] map;
        protected int previousRow;
        protected int previousColumn;
        protected int newRow;
        protected int newColumn;
        protected EventType eventType;
        protected List<Segment> previousSegments;
        protected List<Segment> newSegments;

        public BaseMovementCommand(CaterpillarSimulation Simulation, IServiceManager ServiceManager)
        {
            map = simulation.map;
            simulation = Simulation;
            serviceManager = ServiceManager;
            previousRow = CaterpillarSimulation.caterpillarRow;
            previousColumn = CaterpillarSimulation.caterpillarColumn;
            previousSegments = new List<Segment>(simulation.Caterpillar.Segments);


            //caterpillarRow = CaterpillarSimulation.caterpillarRow;
            //caterpillarColumn = CaterpillarSimulation.caterpillarColumn;

        }

        public abstract Task ExecuteAsync(CaterpillarSimulation simulation);

        public virtual async Task UndoAsync(CaterpillarSimulation simulation)
        {
            CaterpillarSimulation.caterpillarRow = previousRow;
            CaterpillarSimulation.caterpillarColumn = previousColumn;
            map[CaterpillarSimulation.caterpillarRow, CaterpillarSimulation.caterpillarColumn] = 'C';

            // Restore caterpillar segments
            simulation.Caterpillar.Segments = new List<Segment>(previousSegments);

            // Handle specific event type reversion
            switch (eventType)
            {
                case EventType.Obstacle:
                    await serviceManager.CaterpillarService.UnDestroyCaterpillar(map, previousRow, previousColumn);
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
                    break;
                case EventType.Spice:
                    map[previousRow, previousColumn] = 'S';
                    break;
            }
        }
    }
}
