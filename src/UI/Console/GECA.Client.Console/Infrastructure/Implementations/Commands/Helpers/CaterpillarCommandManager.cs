using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Domain.Entities;
using Serilog;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Helpers
{
    public class CaterpillarCommandManager
    {
        private readonly List<ICaterpillarCommand> _commands = new List<ICaterpillarCommand>();
        private readonly string _logFilePath;
        private readonly Stack<ICaterpillarCommand> _undoStack;
        private readonly Stack<ICaterpillarCommand> _redoStack;

        public CaterpillarCommandManager(string logFilePath)
        {
            _logFilePath = logFilePath;
            _undoStack = new Stack<ICaterpillarCommand>();
            _redoStack = new Stack<ICaterpillarCommand>();
        }

        public void ExecuteCommand(ICaterpillarCommand command)
        {
            command.Execute();
            _commands.Add(command);

            _undoStack.Push(command);
            _redoStack.Clear(); // Clear the redo stack whenever a new command is executed

            LogCommand(command);
        }

        public void UndoLastCommand()
        {
            if (_commands.Count > 0)
            {
                var lastCommand = _commands[^1];
                lastCommand.Undo();
                _commands.RemoveAt(_commands.Count - 1);

                LogCommand(lastCommand);

                var command = _undoStack.Pop();
                command.Undo();
                _redoStack.Push(command);

                LogCommand(command);
            }
        }

        public void RedoLastUndoneCommand()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
                LogCommand(command);
            }
        }

        private void LogCommand(ICaterpillarCommand command)
        {
            Log.Information("{DateTime}: {CommandType} executed", DateTime.Now, command.GetType().Name);
            
        }
    }
}
