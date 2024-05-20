using GECA.Client.Console.Application.Dtos;

namespace GECA.Client.Console.Application.Abstractions.IServices
{
    public interface ICaterpillarService
    {
        Task<MoveCaterpillarResponse> MoveCaterpillar(char[,] map, MoveCaterpillarRequest moveCaterpillarRequest);
        Task<int> CollectAndStoreSpice(int row, int column);
        Task<GrowShrinkCaterpillarResponse> GrowShrinkCaterpillar(GrowShrinkCaterpillarRequest growShrinkCaterpillarRequest);
        Task<bool> DestroyCaterpillar(char[,] map, int row, int column);
        Task<bool> UnDestroyCaterpillar(char[,] map, int row, int column);
        Task<bool> IsCaterpillarDestroyed();
        Task<int> GetCaterpillarRow();
        Task<int> GetCaterpillarColumn();
        

    }
}
