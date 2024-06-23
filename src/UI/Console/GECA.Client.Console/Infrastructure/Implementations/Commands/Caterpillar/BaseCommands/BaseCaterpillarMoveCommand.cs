using GECA.Client.Console.Application.Abstractions.ICommand;
using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Application.Dtos;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using Serilog;

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
        protected char previousMapCell;
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
            var testSeg = previousSegments;
            var currentCaterpillarRow = caterpillar.CurrentRow;
            var currentCaterpillarCol = caterpillar.CurrentColumn;

            if (previousSegments == null )
            {
                previousSegments = caterpillar.PreviousSegments ?? caterpillar.Segments;
            }

            switch (eventType)
            {
                case EventType.Moved:
                    map[currentCaterpillarRow, currentCaterpillarCol] = '.';
                    break;
                case EventType.Obstacle:
                    map[currentCaterpillarRow, currentCaterpillarCol] = '#';
                    break;
                case EventType.Booster:
                    map[currentCaterpillarRow, currentCaterpillarCol] = 'B';
                    break;
                case EventType.Spice:
                    map[currentCaterpillarRow, currentCaterpillarCol] = 'S';
                    break;
                case EventType.HorizontalCrossBoundary:
                    map[currentCaterpillarRow, currentCaterpillarCol] = '.';
                    break;
                case EventType.VerticalCrossBoundary:
                    map[currentCaterpillarRow, currentCaterpillarCol] = '.';
                    break;
                case EventType.CrossBoundary:
                    break;
                case EventType.HitMapBoundary:
                    map[currentCaterpillarRow, currentCaterpillarCol] = 'C';
                    break;
                case EventType.None:
                    break;
                default:
                    break;
            }
            

            //RestorePreviousState();

            // Handle specific event type reversion
            switch (eventType)
            {
                case EventType.Moved:
                    RestorePreviousState();

                    break;

                case EventType.Obstacle:

                    await caterpillarService.UnDestroyCaterpillar(map, previousRow, previousColumn);
                    RestorePreviousState();
                    break;

                case EventType.Booster:

                    RestorePreviousState();

                    var growShrinkResponse = new GrowShrinkCaterpillarResponse(); 

                    // Revert the caterpillar size change
                    if (caterpillar.CurrentSegmentCount > previousSegments.Count)
                    {
                        GrowShrinkCaterpillarRequest growShrinkRequest = new()
                        {
                            Caterpillar = new CaterpillarDto { Caterpillar = caterpillar },
                            Grow = false
                        };

                        caterpillar.Segments.RemoveAt(caterpillar.Segments.Count - 1);

                        growShrinkResponse =  await caterpillarService.GrowShrinkCaterpillar(growShrinkRequest);
                    }
                    else if (caterpillar.CurrentSegmentCount < previousSegments.Count)
                    {
                        GrowShrinkCaterpillarRequest growShrinkRequest = new()
                        {
                            Caterpillar = new CaterpillarDto { Caterpillar = caterpillar },
                            Grow = true
                        };

                        caterpillar.Segments.Add(new Segment(SegmentType.Intermediate));

                        growShrinkResponse = await caterpillarService.GrowShrinkCaterpillar(growShrinkRequest);
                    }

                    map[previousRow, previousColumn] = 'C';

                    break;

                case EventType.Spice:
                    RestorePreviousState();
                    break;

                case EventType.HorizontalCrossBoundary:
                case EventType.VerticalCrossBoundary:

                    RestorePreviousState();

                    break;

                case EventType.HitMapBoundary:
                    RestorePreviousState();
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

        public void SaveCurrentState()
        {
            previousRow = caterpillar.CurrentRow;
            previousColumn = caterpillar.CurrentColumn;
            //previousSegments = new List<Segment>(caterpillar.Segments);
            previousMapCell = map[previousRow, previousColumn];
        }

        protected void RestorePreviousState()
        {
            caterpillar.CurrentRow = previousRow;
            caterpillar.CurrentColumn = previousColumn;
            map[previousRow, previousColumn] = previousMapCell;
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
