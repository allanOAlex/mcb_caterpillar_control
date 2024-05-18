using GECA.Client.Console.Application.Abstractions.Intefaces;
using GECA.Client.Console.Application.Abstractions.IServices;
using GECA.Client.Console.Application.Dtos;

namespace GECA.Client.Console.Infrastructure.Implementations.Services
{
    internal sealed class MapService : IMapService
    {
        private readonly IUnitOfWork unitOfWork;
        public MapService(IUnitOfWork UnitOfWork)
        {
            unitOfWork = UnitOfWork;
        }

        public async Task<char[,]> GenerateMapAsync(GenerateMapRequest generateMapRequest)
        {
            try
            {
                // Initialize a new map with the specified size
                char[,] map = new char[generateMapRequest.Size, generateMapRequest.Size];

                // Initialize map with empty spaces
                InitializeMap(map);

                // Place boosters, obstacles, and spices randomly
                PlaceItems(map, generateMapRequest.BoosterCount, 'B'); // 'B' represents boosters
                PlaceItems(map, generateMapRequest.ObstacleCount, '#'); // '#' represents obstacles
                PlaceItems(map, generateMapRequest.SpiceCount, 'S'); // 'S' represents spices

                return await Task.FromResult(map);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<string> GetMapAsString(char[,] map, int size)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetMapSize(char[,] map)
        {
            try
            {
                // Check if the array is null or has zero dimensions
                if (map == null || map.GetLength(0) == 0 || map.GetLength(1) == 0)
                {
                    throw new ArgumentException("Invalid map provided. Size cannot be determined.");
                }

                // Otherwise, return the first dimension size (assuming a square map)
                return map.GetLength(0);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void InitializeMap(char[,] map)
        {
            try
            {
                // Initialize the map with empty spaces
                for (int i = 0; i < map.GetLength(0); i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        map[i, j] = '.';
                    }
                }

                //PlaceCaterpillar(map);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<char[,]> PlaceCaterpillar(char[,] map, int row, int col)
        {
            row = map.GetLength(0) / 2;
            row = map.GetLength(1) / 2;
            map[row, col] = 'C';

            return map;
        }

        public async Task<PlaceCaterpillarResponse> PlaceCaterpillar(char[,] map)
        {
            CaterpillarService caterpillarService = new(unitOfWork);
            var caterpillarRow = await caterpillarService.GetCaterpillarRow();
            var caterpillarColumn = await caterpillarService.GetCaterpillarColumn();

            // Place the caterpillar at the center of the map
            caterpillarRow = map.GetLength(0) / 2;
            caterpillarColumn = map.GetLength(1) / 2;
            map[caterpillarRow, caterpillarColumn] = 'C';

            return new PlaceCaterpillarResponse
            {
                Row = caterpillarRow,
                Column = caterpillarColumn,
            };
        }

        public void PlaceItems(char[,] map, int itemCount, char itemSymbol)
        {
            try
            {
                Random random = new Random();

                int rows = map.GetLength(0);
                int cols = map.GetLength(1);

                for (int i = 0; i < itemCount; i++)
                {
                    int row, col;

                    // Generating random coordinates until an empty cell is found that is not occupied by the caterpillar
                    do
                    {
                        row = random.Next(rows); // Generate random row index within map bounds
                        col = random.Next(cols); // Generate random column index within map bounds

                    } while (map[row, col] != '.');

                    // Check if the cell is already occupied
                    if (map[row, col] == '.')
                    {
                        map[row, col] = itemSymbol;
                    }
                    else
                    {
                        // Retry placing the item at a different location
                        i--; // Decrement i to retry placing the item
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void PrintMap(char[,] map, int size)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(char[,] map, int row, int column)
        {
            // Remove the item from the map
            map[row, column] = '.';
        }

        public void SingleStep_HorizaontalReplicateMapAcrossBoundary(char[,] map, int caterpillarColumn)
        {
            try
            {
                int size = map.GetLength(0);

                // Reflect map's state across the boundary horizontally
                for (int i = 0; i < size; i++)
                {
                    for (int j = 0; j < size / 2; j++)
                    {
                        char temp = map[i, j];
                        map[i, j] = map[i, size - 1 - j];
                        map[i, size - 1 - j] = temp;
                    }
                }

                // Reflect caterpillar's position
                caterpillarColumn = size - 1 - caterpillarColumn;

                // Reflect positions of obstacles, boosters, and spices
                SingleStep_HorizontalReflectItemsAcrossBoundary(map, size);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void SingleStep_VerticalReplicateMapAcrossBoundary(char[,] map, int caterpillarRow)
        {
            try
            {
                // Example vertical mirroring logic (replace with your specific logic)
                for (int i = 0; i < map.GetLength(0) / 2; i++)
                {
                    for (int j = 0; j < map.GetLength(1); j++)
                    {
                        char temp = map[i, j];
                        map[i, j] = map[map.GetLength(0) - 1 - i, j];
                        map[map.GetLength(0) - 1 - i, j] = temp;
                    }
                }

                caterpillarRow = map.GetLength(0) - 1 - caterpillarRow;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void SingleStep_HorizontalReflectItemsAcrossBoundary(char[,] map, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    // Reflect positions of obstacles, boosters, and spices
                    switch (map[i, j])
                    {
                        case '#': // Obstacle
                            map[i, j] = '#'; // No change needed for obstacles
                            break;
                        case 'B': // Booster
                        case 'S': // Spice
                                  // Reflect horizontally
                            map[i, j] = map[i, size - 1 - j];
                            map[i, size - 1 - j] = (map[i, j] == 'B') ? 'B' : 'S'; // Preserve the type
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public async Task<ReplicateMapResponse> SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(ReplicateMapRequest replicateMapRequest)//char[,] map, int caterpillarRow, int caterpillarColumn, bool isHorizontalMirroring
        {
            try
            {
                int size = replicateMapRequest.Map.GetLength(0);

                // Implement mirroring logic based on the flag
                if (replicateMapRequest.IsHorizontalMirroring)
                { 
                    // Reuse existing horizontal mirroring logic
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size / 2; j++)
                        {
                            char temp = replicateMapRequest.Map[i, j];
                            replicateMapRequest.Map[i, j] = replicateMapRequest.Map[i, size - 1 - j];
                            replicateMapRequest.Map[i, size - 1 - j] = temp;
                        }
                    }
                }
                else
                {
                    // Implement vertical mirroring logic
                    for (int i = 0; i < size / 2; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            char temp = replicateMapRequest.Map[i, j];
                            replicateMapRequest.Map[i, j] = replicateMapRequest.Map[size - 1 - i, j];
                            replicateMapRequest.Map[size - 1 - i, j] = temp;
                        }
                    }
                }

                //Reflect caterpillar position based on mirroring direction
                if (replicateMapRequest.IsHorizontalMirroring)
                {
                    replicateMapRequest.CaterpillarColumn = size - 1 - replicateMapRequest.CaterpillarColumn;
                }
                else
                {
                    replicateMapRequest.CaterpillarRow = size - 1 - replicateMapRequest.CaterpillarRow;
                }

                // Reflect positions of obstacles, boosters, and spices
                SingleStep_HorizontalVertical_ReflectItemsAcrossBoundary(replicateMapRequest.Map, size);

                // Calculate new caterpillar position based on mirroring
                int newCaterpillarRow = replicateMapRequest.IsHorizontalMirroring ? size - 1 - replicateMapRequest.CaterpillarColumn : replicateMapRequest.CaterpillarRow;
                int newCaterpillarColumn = replicateMapRequest.IsHorizontalMirroring ? replicateMapRequest.CaterpillarRow : size - 1 - replicateMapRequest.CaterpillarColumn;

                if (!replicateMapRequest.IsHorizontalMirroring)
                {
                    if (newCaterpillarColumn != replicateMapRequest.CaterpillarColumn)
                    {
                        newCaterpillarColumn = replicateMapRequest.CaterpillarColumn;
                    }
                }

                return new ReplicateMapResponse
                {
                    Map = replicateMapRequest.Map,
                    NewCaterpillarRow = newCaterpillarRow,
                    NewCaterpillarColumn = newCaterpillarColumn,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SingleStep_HorizontalVertical_ReflectItemsAcrossBoundary(char[,] map, int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++) 
                {
                    // Reflect positions of obstacles, boosters, and spices
                    switch (map[i, j])
                    {
                        case '#': // Obstacle
                                  // Implement mirroring logic for obstacles
                            if (IsOnHorizontalBoundary(map, i, j, size))
                            {
                                // Horizontal mirroring
                                map[i, j] = map[i, size - 1 - j];
                                map[i, size - 1 - j] = '#'; // Obstacle remains an obstacle
                            }
                            else if (IsOnVerticalBoundary(map, i, j, size))
                            {
                                // Vertical mirroring
                                map[i, j] = map[size - 1 - i, j];
                                map[size - 1 - i, j] = '#'; // Obstacle remains an obstacle
                            }
                            break;
                        case 'B': // Booster
                        case 'S': // Spice
                            if (IsOnHorizontalBoundary(map, i, j, size))
                            {
                                // Horizontal mirroring
                                map[i, j] = map[i, size - 1 - j];
                                map[i, size - 1 - j] = (map[i, j] == 'B') ? 'B' : 'S'; // Preserve the type
                            }
                            else if (IsOnVerticalBoundary(map, i, j, size))
                            {
                                // Vertical mirroring
                                map[i, j] = map[size - 1 - i, j];
                                map[size - 1 - i, j] = (map[i, j] == 'B') ? 'B' : 'S'; // Preserve the type
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private bool IsOnHorizontalBoundary(char[,] map, int i, int j, int size)
        {
            return j == 0 || j == size - 1;
        }

        private bool IsOnVerticalBoundary(char[,] map, int i, int j, int size)
        {
            return i == 0 || i == size - 1;
        }

        
    }
}
