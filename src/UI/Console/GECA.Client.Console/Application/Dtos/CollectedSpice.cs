using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public sealed class CollectedSpice
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public CollectedSpice(int row, int column)
        {
            Row = row;
            Column = column;
        }
    }
}
