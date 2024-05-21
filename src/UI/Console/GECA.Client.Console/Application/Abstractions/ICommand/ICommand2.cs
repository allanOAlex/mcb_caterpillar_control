using GECA.Client.Console.Application.Dtos;

namespace GECA.Client.Console.Application.Abstractions.ICommand
{
    public interface ICommand2
    {
        Task<MoveCaterpillarResponse> ExecuteAsync();
        Task Undo();
        Task Redo();
        void LogCommandDetails(int currentCaterpillarRow, int currentCaterpillarCol);
    }
}