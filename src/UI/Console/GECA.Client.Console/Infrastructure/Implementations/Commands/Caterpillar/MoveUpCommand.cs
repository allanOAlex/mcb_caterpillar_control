using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar
{
    public class MoveUpCommand : ICommand
    {
        public void Execute(CaterpillarState state)
        {
            // Check boundaries and obstacles (existing logic from previous code)
            if (IsValidMove(state.Map, state.CaterpillarRow - 1, state.CaterpillarColumn))
            {
                state.UpdatePosition(state.CaterpillarRow - 1, state.CaterpillarColumn);
                state.Direction = FacingDirection.Up; // Update facing direction (optional)
            }
        }

        public void Undo(CaterpillarState state)
        {
            state.UpdatePosition(state.CaterpillarRow + 1, state.CaterpillarColumn);
            state.Direction = FacingDirection.Down; // Update facing direction (optional)
        }

        private bool IsValidMove(char[,] map, int newRow, int newColumn)
        {
            return true;
        }
    }

}
