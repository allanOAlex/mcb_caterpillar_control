namespace GECA.Client.Console.Application.Dtos
{
    public record LogEntryPropertiesDto : Dto
    {
        public string? Method { get; set; }
        public string? Path { get; set; }
        public string? Time { get; set; }
        public string? RequestId { get; set; }
        public string? RequestPath { get; set; }
        public string? ConnectionId { get; set; }
        public Guid CorrelationId { get; set; }
        public string? MachineName { get; set; }



    }
}
