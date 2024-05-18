namespace GECA.Client.Console.Application.Dtos
{
    public sealed record MoveCaterpillarRequest
    {
        public string? Direction { get; init; }
        public int Steps { get; init; }
        public int CurrentRow { get; set; }
        public int CurrentColumn { get; set; }
    }
}
