using rpsls.Entities.Enums;

namespace rpsls.Entities;

public record MatchResult(
    int ConsecutiveRepeats,
    bool IsNew,
    AttackTypes P1Attack,
    AttackTypes P2Attack,
    ResultTypes Result);