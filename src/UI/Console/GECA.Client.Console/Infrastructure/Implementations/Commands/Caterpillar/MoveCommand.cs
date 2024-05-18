using GECA.Client.Console.Application.Abstractions.ICommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar
{
    internal sealed class MoveCommand : ICommand
    {
        private readonly int oldRow;
        private readonly int oldColumn;
        private readonly int newRow;
        private readonly int newColumn;

        public MoveCommand(int OldRow, int OldColumn, int NewRow, int NewColumn)
        {
            oldRow = OldRow;
            oldColumn = OldColumn;
            newRow = NewRow;
            newColumn = NewColumn;
        }

        public void Execute()
        {
            // Perform the move operation
            // Log the command execution
        }

        public void Redo()
        {
            throw new NotImplementedException();
        }

        public void Undo()
        {
            // Reverse the move operation
            // Log the command undo operation
        }


    }
}
