namespace GECA.Client.Console.Application.Dtos
{
    public record Response
    {
        public bool Successful { get; init; }
        public string Message { get; init; }
    }
}
