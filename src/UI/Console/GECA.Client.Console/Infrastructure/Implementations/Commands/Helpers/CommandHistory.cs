using GECA.Client.Console.Application.Abstractions.ICommand;
using Serilog;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Helpers
{
    public class CommandHistory
    {
        private readonly Stack<ICommand2> _undoStack = new Stack<ICommand2>();
        private readonly Stack<ICommand2> _redoStack = new Stack<ICommand2>();

        public void Execute(ICommand2 command)
        {
            //command.ExecuteAsync().Wait();
            LogCommand(command);
            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Any())
            {
                var command = _undoStack.Pop();
                command.Undo().Wait();
                LogCommand(command);
                _redoStack.Push(command);
            }
        }

        public void Redo()
        {
            if (_redoStack.Any())
            {
                var command = _redoStack.Pop();
                command.Redo().Wait();
                LogCommand(command);
                _undoStack.Push(command);
            }
        }

        private void LogCommand(ICommand2 command)
        {
            Log.Information("{DateTime}: {CommandType} executed", DateTime.Now, command.GetType().Name);

        }
    }
}
