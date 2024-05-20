using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Application.Dtos;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar
{
    public abstract class BaseCaterpillarMovementCommand
    {
        protected Domain.Entities.Caterpillar caterpillar;
        protected char[,] map;
        protected MoveCaterpillarRequest moveCaterpillarRequest;
        protected ICaterpillarService caterpillarService;
        protected IMapService mapService;

        protected int previousRow;
        protected int previousColumn;
        protected List<Segment> previousSegments;

        public BaseCaterpillarMovementCommand(Domain.Entities.Caterpillar caterpillar, char[,] map, MoveCaterpillarRequest MoveCaterpillarRequest, ICaterpillarService caterpillarService, IMapService mapService)
        {
            this.caterpillar = caterpillar;
            this.map = map;
            moveCaterpillarRequest = MoveCaterpillarRequest;
            this.caterpillarService = caterpillarService;
            this.mapService = mapService;
        }

        public abstract Task<MoveCaterpillarResponse> ExecuteAsync();

        protected void SaveCurrentState()
        {
            previousRow = caterpillar.CurrentRow;
            previousColumn = caterpillar.CurrentColumn;
            previousSegments = new List<Segment>(caterpillar.Segments);
        }

        protected void RestorePreviousState()
        {
            caterpillar.CurrentRow = previousRow;
            caterpillar.CurrentColumn = previousColumn;
            caterpillar.Segments = new List<Segment>(previousSegments);
        }
    }
}
