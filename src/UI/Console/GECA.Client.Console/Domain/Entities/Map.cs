namespace GECA.Client.Console.Domain.Entities
{
    public class Map
    {
        public int Size { get; set; }
        public int[,] Grid { get; set; } // Represents the map grid with obstacles, boosters, spices, etc.
        public List<ReplicatedArea> ReplicatedAreas { get; set; }
    }
}
