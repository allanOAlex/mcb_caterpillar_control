using GECA.Client.Console.Domain.Entities;

namespace GECA.Client.Console.Application.Abstractions.ICommand
{
    public interface ICommand
    {
        void Execute(CaterpillarState state);
        void Undo(CaterpillarState state);
    }
}
