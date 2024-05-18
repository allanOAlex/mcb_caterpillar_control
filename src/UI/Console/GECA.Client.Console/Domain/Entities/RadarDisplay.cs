using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Domain.Entities
{
    public class RadarDisplay
    {
        public int Diameter { get; set; }
        public char[,] Display { get; set; } // Represents the radar display grid
    }
}
