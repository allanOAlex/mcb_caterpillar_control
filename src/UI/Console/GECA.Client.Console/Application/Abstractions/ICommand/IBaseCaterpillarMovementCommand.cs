using GECA.Client.Console.Application.Dtos;

namespace GECA.Client.Console.Application.Abstractions.ICommand
{
    public interface IBaseCaterpillarMovementCommand
    {
        Task<MoveCaterpillarResponse> ExecuteAsync();
        Task Undo();
        Task Redo();
        void LogCommandDetails();
    }
}