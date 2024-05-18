using GECA.Client.Console.Application.Dtos;

namespace GECA.Client.Console.Application.Abstractions.IServices
{
    public interface IMapService
    {
        Task<char[,]> GenerateMapAsync(GenerateMapRequest generateMapRequest);
        Task<int> GetMapSize(char[,] map);
        void InitializeMap(char[,] map);
        Task<PlaceCaterpillarResponse> PlaceCaterpillar(char[,] map);
        Task<char[,]> PlaceCaterpillar(char[,] map, int row, int col);
        void PlaceItems(char[,] map, int itemCount, char itemSymbol);
        void PrintMap(char[,] map, int size);
        Task<string> GetMapAsString(char[,] map, int size);
        void RemoveItem(char[,] map,int row, int column);
        void SingleStep_HorizaontalReplicateMapAcrossBoundary(char[,] map, int caterpillarColumn);
        void SingleStep_VerticalReplicateMapAcrossBoundary(char[,] map, int caterpillarRow);
        Task<ReplicateMapResponse> SingleStep_HorizaontalVertical_ReplicateMapAcrossBoundary(ReplicateMapRequest replicateMapRequest);
    }
}
