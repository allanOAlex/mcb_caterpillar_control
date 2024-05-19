using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Abstractions.ICommand
{
    public interface ICommandGeneric
    {
        void Execute();
        void Undo();
    }
}
