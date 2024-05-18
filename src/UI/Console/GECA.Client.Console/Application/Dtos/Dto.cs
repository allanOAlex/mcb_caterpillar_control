namespace GECA.Client.Console.Application.Dtos
{
    public record Dto
    {
        public int Id { get; set; }
        public bool Succesful { get; set; }
        public string? Message { get; set; }

    }
}
