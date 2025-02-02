namespace rpsls.Infrastructure.Dtos
{
    internal class GameDto
    {
        public int BestOf { get; init; }
        public long PkGameId { get; init; }
        public int Player1 { get; init; }
        public int Player2 { get; init; }
    }
}