using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public class DestroyCaterpillarRequest
    {
        public int PrevousRow { get; set; }
        public int PrevousRColumn { get; set; }
        public int CurrentRow { get; set; }
        public int CurrentColumn { get; set; }
    }
}
