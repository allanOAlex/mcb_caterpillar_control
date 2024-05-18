using GECA.Client.Console.Application.Abstractions.IRepositories;

namespace GECA.Client.Console.Application.Abstractions.Intefaces
{
    public interface IUnitOfWork
    {
        ICaterpillarRepository CaterpillarRepository { get; }
        ISpiceRepository SpiceRepository { get; }

        Task<int> CompleteAsync();
    }
}
