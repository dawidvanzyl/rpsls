using rpsls.Entities.Enums;

namespace rpsls.Entities;

public record Match(
    int ConsecutiveRepeats,
    bool IsNew,
    AttackTypes P1Attack,
    AttackTypes P2Attack,
    ResultTypes Result,
    int Round);