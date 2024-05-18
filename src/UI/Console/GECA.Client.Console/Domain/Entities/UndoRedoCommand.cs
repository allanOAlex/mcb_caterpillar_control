using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Domain.Entities
{
    public class UndoRedoCommand
    {
        public string ActionType { get; set; } // Undo or Redo
        public Command Command { get; set; } // The command to be undone or redone
    }
}
