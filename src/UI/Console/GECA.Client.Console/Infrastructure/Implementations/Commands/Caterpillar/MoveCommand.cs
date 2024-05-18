using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar
{
    public class MoveCommand : ICaterpillarCommand
    {
        private readonly MoveCaterpillarRequest _moveCaterpillarRequest;
        private readonly char[,] _map;
        private readonly int _initialRow;
        private readonly int _initialColumn;

        private int _previousRow;
        private int _previousColumn;

        private readonly char[,] _initialMapState;

        public MoveCommand(MoveCaterpillarRequest moveCaterpillarRequest, char[,] map, int initialRow, int initialColumn)
        {
            _moveCaterpillarRequest = moveCaterpillarRequest;
            _map = map;
            _initialRow = initialRow;
            _initialColumn = initialColumn;

            // Store a copy of the initial state of the map
            _initialMapState = new char[map.GetLength(0), map.GetLength(1)];
            Array.Copy(map, _initialMapState, map.Length);

        }

        public void Execute()
        {
            // Store the current position before executing the move
            _previousRow = _moveCaterpillarRequest.CurrentRow;
            _previousColumn = _moveCaterpillarRequest.CurrentColumn;

            // Update the current position based on the direction and steps
            switch (_moveCaterpillarRequest.Direction.ToUpper())
            {
                case "UP":
                case "U":
                    _moveCaterpillarRequest.CurrentRow -= _moveCaterpillarRequest.Steps;
                    break;
                case "DOWN":
                case "D":
                    _moveCaterpillarRequest.CurrentRow += _moveCaterpillarRequest.Steps;
                    break;
                case "LEFT":
                case "L":
                    _moveCaterpillarRequest.CurrentColumn -= _moveCaterpillarRequest.Steps;
                    break;
                case "RIGHT":
                case "R":
                    _moveCaterpillarRequest.CurrentColumn += _moveCaterpillarRequest.Steps;
                    break;
                default:
                    // Handle invalid direction
                    break;
            }

            // Ensure the new position is within the map boundaries
            // Check if the new row is within the bounds of the map array
            if (_moveCaterpillarRequest.CurrentRow < 0 || _moveCaterpillarRequest.CurrentRow >= _map.GetLength(0))
            {
                // Adjust the row to keep it within bounds
                _moveCaterpillarRequest.CurrentRow = Math.Clamp(_moveCaterpillarRequest.CurrentRow, 0, _map.GetLength(0) - 1);
            }

            // Check if the new column is within the bounds of the map array
            if (_moveCaterpillarRequest.CurrentColumn < 0 || _moveCaterpillarRequest.CurrentColumn >= _map.GetLength(1))
            {
                // Adjust the column to keep it within bounds
                _moveCaterpillarRequest.CurrentColumn = Math.Clamp(_moveCaterpillarRequest.CurrentColumn, 0, _map.GetLength(1) - 1);
            }
        }

        public void Undo()
        {
            // Logic to undo the move operation and restore the previous position of the caterpillar and map
            _moveCaterpillarRequest.CurrentRow = _previousRow;
            _moveCaterpillarRequest.CurrentColumn = _previousColumn;

            // Restore the previous state of the map
            for (int i = 0; i < _map.GetLength(0); i++)
            {
                for (int j = 0; j < _map.GetLength(1); j++)
                {
                    _map[i, j] = _initialMapState[i, j];
                }
            }
        }
    }
}
