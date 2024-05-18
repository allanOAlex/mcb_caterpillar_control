using GECA.Client.Console.Application.Abstractions.IServices;

namespace GECA.Client.Console.Application.Abstractions.Intefaces
{
    public interface IServiceManager
    {
        ICaterpillarService CaterpillarService { get; }
        IMapService MapService { get; }
    }
}
