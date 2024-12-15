using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms
{
    public class CustomAlgorithm : IAlgorithm
    {
        public AttackTypes CalculateAttack()
        {
            return (AttackTypes)Random.Shared.Next(1, 3);
        }
    }
}