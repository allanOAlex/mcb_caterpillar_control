using GECA.Client.Console.Application.Abstractions.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Abstractions.Intefaces
{
    public interface IUnitOfWork
    {
        ICaterpillarRepository CaterpillarRepository { get; }
        ISpiceRepository SpiceRepository { get; }

        Task<int> CompleteAsync();
    }
}
