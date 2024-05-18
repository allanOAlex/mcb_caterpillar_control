using GECA.Client.Console.Application.Abstractions.ICommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar
{
    public class PlaceCaterpillarOnMapCommand : ICommand
    {
        public char[,] Map { get; private set; }
        public int CurrentRow { get; private set; }
        public int PreviousRow { get; private set; }
        public int CurrentColumn { get; private set; }
        public int PreviousColumn { get; private set; }
        public string Direction { get; private set; }
        public int Steps { get; private set; }
        

        public PlaceCaterpillarOnMapCommand(char[,] map, int currentRow, int currentColumn, int previousRow, int previousColumn, string direction, int steps)
        {
            Map = map;
            CurrentRow = currentRow;
            CurrentColumn = currentColumn;
            PreviousRow = previousRow;
            PreviousColumn = previousColumn;
            Direction = direction;
            Steps = steps;
        }

        public void Execute()
        {
            // Save the previous position of the caterpillar
            PreviousRow = CurrentRow;
            PreviousColumn = CurrentColumn;

            // Place the caterpillar at the center of the map
            CurrentRow = Map.GetLength(0) / 2;
            CurrentColumn = Map.GetLength(1) / 2;
            Map[CurrentRow, CurrentColumn] = 'C';
        }

        public void Undo()
        {
            // Remove the caterpillar from its current position
            Map[CurrentRow, CurrentColumn] = ' ';

            // Place the caterpillar back to its previous position
            CurrentRow = PreviousRow;
            CurrentColumn = PreviousColumn;
            Map[CurrentRow, CurrentColumn] = 'C';
        }

        public void Redo()
        {
            // Execute the placement again (same as Execute method)
            Execute();
        }
    }
}
