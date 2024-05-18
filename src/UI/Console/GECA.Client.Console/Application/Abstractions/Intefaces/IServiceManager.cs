using GECA.Client.Console.Application.Abstractions.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Abstractions.Intefaces
{
    public interface IServiceManager
    {
        ICaterpillarService CaterpillarService { get; }
        IMapService MapService { get; }
    }
}
