using GECA.Client.Console.Domain.Enums;

namespace GECA.Client.Console.Domain.Entities
{
    public class CaterpillarState
    {
        public char[,] Map { get; set; }
        public int CaterpillarRow { get; set; }
        public int CaterpillarColumn { get; set; }
        public FacingDirection Direction { get; set; } // Optional facing direction

        public void UpdatePosition(int newRow, int newColumn)
        {
            CaterpillarRow = newRow;
            CaterpillarColumn = newColumn;
        }

        public void CloneState(CaterpillarState other)
        {
            Map = other.Map.Clone() as char[,];
            CaterpillarRow = other.CaterpillarRow;
            CaterpillarColumn = other.CaterpillarColumn;
            Direction = other.Direction;
        }
    }
}
