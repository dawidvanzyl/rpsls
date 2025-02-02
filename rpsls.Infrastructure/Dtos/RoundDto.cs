namespace rpsls.Infrastructure.Dtos;

public class RoundDto
{
    public int ConsecutiveRepeats { get; init; }
    public long FkGameId { get; init; }
    public int Player1 { get; init; }
    public int Player2 { get; init; }
    public int Result { get; init; }
    public int Round { get; init; }
}