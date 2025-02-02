using rpsls.Entities.Enums;

namespace rpsls.Entities;

public record Round(
    int Number,
    AttackTypes P1Attack,
    AttackTypes P2Attack,
    ResultTypes Result,
    int ConsecutiveRepeats);