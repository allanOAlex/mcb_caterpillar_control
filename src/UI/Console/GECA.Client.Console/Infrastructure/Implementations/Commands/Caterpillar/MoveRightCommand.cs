using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar
{
    public class MoveRightCommand : ICommand
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
        public void Redo(CaterpillarState state) 
        {  

        }

    }
}
