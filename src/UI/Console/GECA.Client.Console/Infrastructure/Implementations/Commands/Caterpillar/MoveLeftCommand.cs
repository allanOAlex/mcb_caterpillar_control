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
    public class MoveLeftCommand : ICommand 
    {
        public void Execute(CaterpillarState state) 
        {
            if (IsValidMove(state.Map, state.CaterpillarRow - 1, state.CaterpillarColumn))
            {
                state.UpdatePosition(state.CaterpillarRow - 1, state.CaterpillarColumn);
                state.Direction = FacingDirection.Up;
            }
        }

        private bool IsValidMove(char[,] map, int v, int caterpillarColumn)
        {
            return true;

        }

        public void Undo(CaterpillarState state) 
        {
            state.UpdatePosition(state.CaterpillarRow + 1, state.CaterpillarColumn);
            state.Direction = FacingDirection.Down;
        }
    }

}
