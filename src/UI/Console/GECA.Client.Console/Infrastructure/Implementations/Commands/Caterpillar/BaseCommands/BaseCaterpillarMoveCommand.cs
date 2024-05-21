using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Application.Dtos;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GECA.Client.Console.Infrastructure.Implementations.Commands.Caterpillar.BaseCommands
{
    public abstract class BaseCaterpillarMoveCommand : ICommand2
    {
        protected Domain.Entities.Caterpillar caterpillar;
        protected char[,] map;
        protected MoveCaterpillarRequest moveCaterpillarRequest;
        protected ICaterpillarService caterpillarService;
        protected IMapService mapService;
        protected EventType eventType;
        protected int previousRow;
        protected int previousColumn;
        protected List<Segment> previousSegments;
        protected List<Segment> newSegments;

        public BaseCaterpillarMoveCommand(Domain.Entities.Caterpillar caterpillar, char[,] map, MoveCaterpillarRequest MoveCaterpillarRequest, ICaterpillarService caterpillarService, IMapService mapService)
        {
            this.caterpillar = caterpillar;
            this.map = map;
            moveCaterpillarRequest = MoveCaterpillarRequest;
            this.caterpillarService = caterpillarService;
            this.mapService = mapService;
        }

        public abstract Task<MoveCaterpillarResponse> ExecuteAsync();

        public async Task Undo()
        {

            var currentCaterpillarRow = caterpillar.CurrentRow;
            var currentCaterpillarCol = caterpillar.CurrentColumn;
            map[currentCaterpillarRow, currentCaterpillarCol] = '.';

            RestorePreviousState();

            // Handle specific event type reversion
            switch (eventType)
            {
                case EventType.Moved:
                    // Restore caterpillar position
                    CaterpillarSimulation.caterpillarRow = previousRow;
                    CaterpillarSimulation.caterpillarColumn = previousColumn;
                    map[CaterpillarSimulation.caterpillarRow, CaterpillarSimulation.caterpillarColumn] = 'C';

                    // Update current position with empty sapce

                    // Restore caterpillar segments
                    caterpillar.Segments = new List<Segment>(previousSegments);

                    break;

                case EventType.Obstacle:
                    await caterpillarService.UnDestroyCaterpillar(map, previousRow, previousColumn);

                    break;
                case EventType.Booster:
                    // Revert the caterpillar size change
                    if (previousSegments.Count > newSegments.Count)
                    {
                        caterpillar.Segments.RemoveAt(caterpillar.Segments.Count - 1);
                    }
                    else if (previousSegments.Count < newSegments.Count)
                    {
                        caterpillar.Segments.Add(new Segment(SegmentType.Intermediate));
                    }
                    map[previousRow, previousColumn] = 'B';
                    break;

                case EventType.Spice:
                    map[previousRow, previousColumn] = 'S';
                    break;

                case EventType.HorizontalCrossBoundary:
                case EventType.VerticalCrossBoundary:
                    
                    // Handle reverting boundary crossing by replicating the map back to its original state
                    await mapService.SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(new ReplicateMapRequest
                    {
                        Map = map,
                        CaterpillarRow = previousRow,
                        CaterpillarColumn = previousColumn,
                        IsHorizontalMirroring = eventType == EventType.HorizontalCrossBoundary ? true : false,
                    });
                    break;

                case EventType.HitMapBoundary:
                    // Simply restoring the previous state might be enough
                    break;
            }

            Log.Information("Undo executed for {CommandType}. Caterpillar Position reverted to: ({Row}, {Column}), Segments: {SegmentCount}",
                GetType().Name, caterpillar.CurrentRow, caterpillar.CurrentColumn, caterpillar.Segments.Count);
            await Task.CompletedTask;
        }

        public async Task Redo()
        {
            await ExecuteAsync();
            Log.Information("Redo executed for {CommandType}. Caterpillar Position: ({Row}, {Column}), Segments: {SegmentCount}",
                GetType().Name, caterpillar.CurrentRow, caterpillar.CurrentColumn, caterpillar.Segments.Count);
        }

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

        public void LogCommandDetails(int currentCaterpillarRow, int currentCaterpillarCol)
        {
            Log.Information("" +
                "{DateTime}: {CommandType} executed. " +
                "Previous Caterpillar Position: ({CurrentRow}, {CurrentColumn}), " +
                "Current Caterpillar Position: ({CurrentRow}, {CurrentColumn}), " +
                "Segments: {SegmentCount}",
                DateTime.Now, GetType().Name,
                caterpillar.PreviousRow, caterpillar.PreviousColumn,
                caterpillar.CurrentRow, caterpillar.CurrentColumn,
                caterpillar.Segments.Count);
        }
    }
}
