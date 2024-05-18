namespace GECA.Client.Console.Application.Dtos
{
    public record ReplicateMapRequest
    {
        public char[,] Map { get; init; }
        public int CaterpillarRow { get; set; }
        public int CaterpillarColumn { get; set; }
        public bool IsHorizontalMirroring { get; init; }
    }
}
