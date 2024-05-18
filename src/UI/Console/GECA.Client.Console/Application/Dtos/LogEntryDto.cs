namespace GECA.Client.Console.Application.Dtos
{
    public record LogEntryDto : Dto
    {
        public DateTime Timestamp { get; set; }
        public string? Level { get; set; }
        public string? MessageTemplate { get; set; }
        public LogEntryPropertiesDto? Properties { get; set; }


    }
}
