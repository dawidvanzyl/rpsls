using rpsls.Entities.Enums;

namespace rpsls.Entities
{
    public record MatchResult(
        int AttackCount,
        bool IsNew,
        AttackTypes Player1,
        AttackTypes Player2,
        ResultTypes Result);
}