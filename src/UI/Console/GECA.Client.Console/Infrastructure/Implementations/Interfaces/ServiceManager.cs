using GECA.Client.Console.Application.Abstractions.Intefaces;
using GECA.Client.Console.Application.Abstractions.IServices;

namespace GECA.Client.Console.Infrastructure.Implementations.Interfaces
{
    internal class ServiceManager : IServiceManager
    {
        public ICaterpillarService CaterpillarService { get; private set; }
        public IMapService MapService { get; private set; }


        public ServiceManager(ICaterpillarService caterpillarService, IMapService mapService)
        {
            CaterpillarService = caterpillarService;
            MapService = mapService;
        }


    }
}
