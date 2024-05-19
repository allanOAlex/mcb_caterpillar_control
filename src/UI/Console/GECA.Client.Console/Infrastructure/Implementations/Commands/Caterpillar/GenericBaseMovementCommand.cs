using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar
{
    public abstract class GenericBaseMovementCommand : ICommandGeneric
    {
        protected readonly char[,] map;
        protected readonly int previousRow;
        protected readonly int previousColumn;
        protected readonly List<Segment> previousSegments;
        protected readonly CaterpillarSimulation simulation;

        public GenericBaseMovementCommand(CaterpillarSimulation simulation)
        {
            this.simulation = simulation;
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
        }
    }
}
