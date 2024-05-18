namespace GECA.Client.Console.Application.Dtos
{
    public record GrowShrinkCaterpillarResponse : Response
    {
        public bool CaterpillarGrown { get; set; }
        public bool CaterpillarShrunk { get; set; }
        public int InitialSegments { get; set; }
        public int CurrentSegments { get; set; }
    }
}
