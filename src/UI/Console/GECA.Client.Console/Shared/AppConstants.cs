using GECA.Client.Console.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Shared
{
    public static class AppConstants
    {
        public static int CurrentCaterpillarRow { get; set; }
        public static int CurrentCaterpillarColumn { get; set; }
        public static int RadarRange { get; set; } = 11;
        public static List<Spice> SpiceList { get; set; }
    }
}
