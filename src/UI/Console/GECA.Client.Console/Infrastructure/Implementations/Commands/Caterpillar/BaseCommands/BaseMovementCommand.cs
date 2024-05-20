using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Application.Abstractions.Intefaces;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.BaseCommands
{
    public abstract class BaseMovementCommand : ICommand
    {
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

        public BaseMovementCommand(CaterpillarSimulation simulation, IServiceManager serviceManager)
        {
            this.simulation = simulation;
            this.serviceManager = serviceManager;
            map = simulation.map;
            previousRow = CaterpillarSimulation.caterpillarRow;
            previousColumn = CaterpillarSimulation.caterpillarColumn;
            previousSegments = new List<Segment>(simulation.Caterpillar.Segments);
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

            // Log the undo action
            LogCommand(GetType().Name, eventType);
        }

        protected void SaveCurrentState()
        {
            previousRow = CaterpillarSimulation.caterpillarRow;
            previousColumn = CaterpillarSimulation.caterpillarColumn;
            previousSegments = new List<Segment>(simulation.Caterpillar.Segments);
        }

        protected void RestorePreviousState()
        {
            CaterpillarSimulation.caterpillarRow = previousRow;
            CaterpillarSimulation.caterpillarColumn = previousColumn;
            simulation.Caterpillar.Segments = new List<Segment>(previousSegments);
        }

        protected void LogCommand(string commandType, EventType eventType)
        {
            Log.Information("{DateTime}: {CommandType} executed with event {EventType} at ({PreviousRow}, {PreviousColumn}) to ({NewRow}, {NewColumn}), Segments: {SegmentCount}",
                DateTime.Now, commandType, eventType, previousRow, previousColumn, newRow, newColumn, simulation.Caterpillar.Segments.Count);
        }
    }
}
