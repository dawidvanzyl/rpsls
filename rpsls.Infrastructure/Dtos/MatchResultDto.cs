using rpsls.Entities.Enums;

namespace rpsls.Infrastructure.Dtos;

public class MatchResultDto
{
    public int AttackCount { get; init; }
    public AttackTypes Player1 { get; init; }
    public AttackTypes Player2 { get; init; }
    public ResultTypes Result { get; init; }
}