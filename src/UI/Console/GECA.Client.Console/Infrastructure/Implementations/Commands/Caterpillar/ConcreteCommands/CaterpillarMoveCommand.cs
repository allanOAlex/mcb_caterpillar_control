using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Application.Dtos;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.BaseCommands;
using GECA.Client.Console.Shared;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.ConcreteCommands
{
    public class CaterpillarMoveCommand : BaseCaterpillarMoveCommand
    {
        public CaterpillarMoveCommand(Domain.Entities.Caterpillar caterpillar, char[,] map, MoveCaterpillarRequest moveCaterpillarRequest, ICaterpillarService caterpillarService, IMapService mapService)
        : base(caterpillar, map, moveCaterpillarRequest, caterpillarService, mapService)
        {
        }

        public override async Task<MoveCaterpillarResponse> ExecuteAsync()
        {
            SaveCurrentState();

            var moveResponse = await caterpillarService.MoveCaterpillar(map, moveCaterpillarRequest);

            if (moveResponse == null)
            {
                throw new InvalidOperationException("MoveCaterpillarResponse cannot be null");
            }

            caterpillar.PreviousRow = moveCaterpillarRequest.CurrentRow;
            caterpillar.PreviousColumn = moveCaterpillarRequest.CurrentColumn;
            caterpillar.CurrentRow = moveResponse.NewCatapillarRow;
            caterpillar.CurrentColumn = moveResponse.NewCatapillarColumn;

            switch (moveResponse.EventType)
            {
                case EventType.Obstacle:
                    caterpillar.Segments.Clear();
                    caterpillar.Segments.Add(new Segment(SegmentType.Head));
                    caterpillar.Segments.Add(new Segment(SegmentType.Tail));
                    await caterpillarService.DestroyCaterpillar(map, moveCaterpillarRequest.CurrentRow, moveCaterpillarRequest.CurrentColumn);
                    break;

                case EventType.Booster:
                  
                    //Best handled in CaterpillarSimulation class
                    break;

                case EventType.Spice:
                    await caterpillarService.CollectAndStoreSpice(moveResponse.NewCatapillarRow, moveResponse.NewCatapillarColumn);
                    
                    break;

                case EventType.HorizontalCrossBoundary:
                case EventType.VerticalCrossBoundary:
                    await mapService.SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(new ReplicateMapRequest
                    {
                        Map = map,
                        CaterpillarRow = moveResponse.NewCatapillarRow,
                        CaterpillarColumn = moveResponse.NewCatapillarColumn,
                        IsHorizontalMirroring = moveResponse.EventType == EventType.HorizontalCrossBoundary ? true : false
                    });

                    await HandleBoundaryCross(moveResponse);

                    break;

                case EventType.HitMapBoundary:
                    HandleMapBoundaryHit();

                    break;
            }

            LogCommandDetails(moveResponse.NewCatapillarRow, moveResponse.NewCatapillarColumn);

            return moveResponse;
        }


        private async Task HandleBoundaryCross(MoveCaterpillarResponse moveResponse)
        {
            var replicateMapRequest = new ReplicateMapRequest
            {
                Map = map,
                CaterpillarRow = moveResponse.NewCatapillarRow,
                CaterpillarColumn = moveResponse.NewCatapillarColumn,
                IsHorizontalMirroring = moveResponse.EventType == EventType.HorizontalCrossBoundary ? false : true  
            };

            await mapService.SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(replicateMapRequest);

            //// Update the caterpillar's position based on the new map after crossing the boundary
            //caterpillar.CurrentRow = moveResponse.NewCatapillarRow;
            //caterpillar.CurrentColumn = moveResponse.NewCatapillarColumn;

            Log.Information("{DateTime}: Caterpillar crossed boundary. New Position: ({Row}, {Column})",
                DateTime.Now, caterpillar.CurrentRow, caterpillar.CurrentColumn);
        }

        private void HandleMapBoundaryHit()
        {
            // Caterpillar hits the map boundary, so we reset it to the previous position
            RestorePreviousState();
            map[previousRow, previousColumn] = 'C';

            Log.Information("{DateTime}: Caterpillar hit map boundary. Reset to Previous Position: ({Row}, {Column})",
                DateTime.Now, caterpillar.CurrentRow, caterpillar.CurrentColumn);
        }

    }
}
