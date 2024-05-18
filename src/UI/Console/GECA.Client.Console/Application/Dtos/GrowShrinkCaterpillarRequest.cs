using GECA.Client.Console.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Application.Dtos
{
    public record GrowShrinkCaterpillarRequest
    {
        public CaterpillarDto Caterpillar { get; init; }
        public bool Grow { get; init; }
    }
}
