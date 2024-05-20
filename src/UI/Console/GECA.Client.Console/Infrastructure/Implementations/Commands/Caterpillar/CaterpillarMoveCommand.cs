﻿using GECA.Client.Console.Application.Abstractions.IServices;
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
    public class CaterpillarMoveCommand : BaseCaterpillarMovementCommand
    {
        public CaterpillarMoveCommand(Domain.Entities.Caterpillar caterpillar, char[,] map, MoveCaterpillarRequest moveCaterpillarRequest, ICaterpillarService caterpillarService, IMapService mapService)
        : base(caterpillar, map, moveCaterpillarRequest, caterpillarService, mapService)
        {
        }

        public override async Task<MoveCaterpillarResponse> ExecuteAsync()
        {
            SaveCurrentState();

            //var moveResponse = await caterpillarService.MoveCaterpillar(map, new MoveCaterpillarRequest
            //{
            //    Direction = direction,
            //    Steps = steps
            //});

            var moveResponse = await caterpillarService.MoveCaterpillar(map, moveCaterpillarRequest);

            switch (moveResponse.EventType)
            {
                case EventType.Obstacle:
                    await caterpillarService.DestroyCaterpillar(map, moveResponse.NewCatapillarRow, moveResponse.NewCatapillarColumn);
                    break;
                case EventType.Booster:
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
                    break;
            }

            return moveResponse;
        }
    }
}