namespace GECA.Client.Console.Application.Dtos
{
    public record QueryLogFileRequest
    {
        public string? LogFile { get; set; }
        public string? Level { get; set; }
        public string? Method { get; set; }
        public string? Time { get; set; }
        public string? RequestId { get; set; }
        public string? RequestPath { get; set; }
        public string? ConnectionId { get; set; }
        public string? CorrelationId { get; set; }



    }
}
