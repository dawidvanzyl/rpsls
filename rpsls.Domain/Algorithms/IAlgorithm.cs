using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms;

public interface IAlgorithm
{
    AttackTypes CalculateAttack();
}