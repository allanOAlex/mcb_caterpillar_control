using GECA.Client.Console.Application.Abstractions.Intefaces;
using GECA.Client.Console.Application.Abstractions.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
