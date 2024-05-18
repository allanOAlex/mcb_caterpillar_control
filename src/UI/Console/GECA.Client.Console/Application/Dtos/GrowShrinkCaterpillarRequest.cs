namespace GECA.Client.Console.Application.Dtos
{
    public record GrowShrinkCaterpillarRequest
    {
        public CaterpillarDto Caterpillar { get; init; }
        public bool Grow { get; init; }
    }
}
