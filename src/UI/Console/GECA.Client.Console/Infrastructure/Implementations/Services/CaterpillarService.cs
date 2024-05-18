using GECA.Client.Console.Application.Abstractions.Intefaces;
using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Application.Dtos;
using GECA.Client.Console.Domain.Entities;
using GECA.Client.Console.Domain.Enums;
using GECA.Client.Console.Shared;

namespace GECA.Client.Console.Infrastructure.Implementations.Services
{
    internal sealed class CaterpillarService : ICaterpillarService
    {
        private readonly int mapSize;
        private readonly char[,] map;
        private static int caterpillarRow;
        private static int caterpillarColumn;
        private bool caterpillarDestroyed; // Flag to indicate if caterpillar is destroyed
        private List<CollectedSpice> CollectedSpices; // Collection to store encountered spices
        private List<Spice> SpiceList; // Collection to store encountered spices
        

        private readonly IUnitOfWork unitOfWork;


        public CaterpillarService(IUnitOfWork UnitOfWork)
        {
            map = new char[mapSize, mapSize];
            CollectedSpices = new ();
            SpiceList = new();
            caterpillarDestroyed = false;
            unitOfWork = UnitOfWork;
            
        }


        public async Task<bool> DestroyCaterpillar(char[,] map, int row, int column)
        {
            try
            {
                caterpillarDestroyed = true;

                // Remove the caterpillar from the map
                map[row, column] = '.';

                return true;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task<int> GetCaterpillarColumn()
        {
            return caterpillarColumn;
        }

        public async Task<int> GetCaterpillarRow()
        {
            return caterpillarRow;
        }

        public async Task<bool> IsCaterpillarDestroyed()
        {
            return caterpillarDestroyed;
        }

        public async Task<MoveCaterpillarResponse> MoveCaterpillar(char[,] map, MoveCaterpillarRequest moveCaterpillarRequest)
        {
            try
            {
                
                int newRow = moveCaterpillarRequest.CurrentRow;
                int newColumn = moveCaterpillarRequest.CurrentColumn;

                // Update new position based on the direction and steps
                switch (moveCaterpillarRequest.Direction!.ToUpper())
                {
                    case "UP":
                    case "U":
                        newRow -= moveCaterpillarRequest.Steps;
                        break;
                    case "DOWN":
                    case "D":
                        newRow += moveCaterpillarRequest.Steps;
                        break;
                    case "LEFT":
                    case "L":
                        newColumn -= moveCaterpillarRequest.Steps;
                        break;
                    case "RIGHT":
                    case "R":
                        newColumn += moveCaterpillarRequest.Steps;
                        break;
                    default:
                        return new MoveCaterpillarResponse { Successful = false, Message = "Invalid Direction." };
                }

                // Check if the new position is within the map boundaries
                if (newRow < 0 || newRow >= map.GetLength(0) || newColumn < 0 || newColumn >= map.GetLength(1))
                {
                    return new MoveCaterpillarResponse { Successful = false, Message = "Cannot move. Reached map boundary.", EventType = EventType.HitMapBoundary };
                }

                // Check if the new position has an obstacle
                if (map[newRow, newColumn] == '#')
                {
                    return new MoveCaterpillarResponse { Successful = false, Message = "Hit an obstacle!", EventType = EventType.Obstacle };
                }

                // Clear the current position of the caterpillar
                map[moveCaterpillarRequest.CurrentRow, moveCaterpillarRequest.CurrentColumn] = '.';

                // Update caterpillar's position
                moveCaterpillarRequest.CurrentRow = newRow;
                moveCaterpillarRequest.CurrentColumn = newColumn;

                // Check if the caterpillar encountered any items
                char item = map[moveCaterpillarRequest.CurrentRow, moveCaterpillarRequest.CurrentColumn];
                switch (item)
                {
                    case 'B':
                        return new MoveCaterpillarResponse { Successful = true, Message = "Found a booster!", EventType = EventType.Booster, NewCatapillarRow = moveCaterpillarRequest.CurrentRow, NewCatapillarColumn = moveCaterpillarRequest.CurrentColumn };

                    case 'S':
                        return new MoveCaterpillarResponse { Successful = true, Message = "Found a spice!", EventType = EventType.Spice, NewCatapillarRow = moveCaterpillarRequest.CurrentRow, NewCatapillarColumn = moveCaterpillarRequest.CurrentColumn };

                }

                if (newRow == 0 || newRow == map.GetLength(0) - 1 || newColumn == 0 || newColumn == map.GetLength(1) - 1)
                {
                    // Boundary crossing detected
                    EventType eventType = (newRow == 0 || newRow == map.GetLength(0) - 1) ? EventType.VerticalCrossBoundary : EventType.HorizontalCrossBoundary;
                    return new MoveCaterpillarResponse { Successful = true, Message = "Crossing Bounderies!", EventType = eventType, NewCatapillarRow = moveCaterpillarRequest.CurrentRow, NewCatapillarColumn = moveCaterpillarRequest.CurrentColumn };
                }

                // Update caterpillar's new position on the map
                map[moveCaterpillarRequest.CurrentRow, moveCaterpillarRequest.CurrentColumn] = 'C';

                return new MoveCaterpillarResponse { Successful = true, Message = "Move Successful!", EventType = EventType.Moved, NewCatapillarRow = moveCaterpillarRequest.CurrentRow, NewCatapillarColumn = moveCaterpillarRequest.CurrentColumn };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<GrowShrinkCaterpillarResponse> GrowShrinkCaterpillar(GrowShrinkCaterpillarRequest growShrinkCaterpillarRequest)
        {
            try
            {
                var initialSegments = growShrinkCaterpillarRequest.Caterpillar.Caterpillar.Segments.Count;

                if (growShrinkCaterpillarRequest.Grow)
                {
                    // Check if the caterpillar can grow (maximum 5 segments)
                    if (initialSegments < 5)
                    {
                        // Add a new segment to the caterpillar
                        growShrinkCaterpillarRequest.Caterpillar.Caterpillar.Segments.Insert(1, new Segment(SegmentType.Intermediate)); // Insert after the head
                        return new GrowShrinkCaterpillarResponse { Successful = true, CaterpillarGrown = true, InitialSegments = initialSegments, CurrentSegments = growShrinkCaterpillarRequest.Caterpillar.Caterpillar.Segments.Count };
                    }
                    else
                    {
                        // Caterpillar is already at maximum size
                        return new GrowShrinkCaterpillarResponse { Successful = false };
                    }
                }
                else
                {
                    // Check if the caterpillar can shrink (minimum 2 segments)
                    if (initialSegments > 2)
                    {
                        // Remove the last segment from the caterpillar
                        growShrinkCaterpillarRequest.Caterpillar.Caterpillar.Segments.RemoveAt(growShrinkCaterpillarRequest.Caterpillar.Caterpillar.Segments.Count - 1);
                        return new GrowShrinkCaterpillarResponse { Successful = true, CaterpillarShrunk = true, InitialSegments = initialSegments, CurrentSegments = growShrinkCaterpillarRequest.Caterpillar.Segments.Count };
                    }
                    else
                    {
                        // Caterpillar is already at minimum size
                        return new GrowShrinkCaterpillarResponse { Successful = false };
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> CollectAndStoreSpice(int row, int column)
        {
            try
            {
                // Store the encountered spice
                CollectedSpices.Add(new CollectedSpice(row, column));
                
                foreach (var spice in CollectedSpices)
                {
                    Spice spiceToSave = new()
                    {
                        Row = spice.Row,
                        Column = spice.Column,
                    };

                    SpiceList.Add(spiceToSave);
                    

                    var savedSpice = await unitOfWork.SpiceRepository.Create(spiceToSave);
                    if (savedSpice != null)
                        SpiceList.Add(spiceToSave);
                }

                if (SpiceList.Count > 0)
                {
                    AppConstants.SpiceList = SpiceList;
                    return CollectedSpices.Count;
                }

                return 0;
            }
            catch (Exception)
            {

                throw;
            }
        }

        
    }
}
