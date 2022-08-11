using rpsls.Application;
using rpsls.Domain;

namespace rpsls.Infrastructure.Algorithms
{
    public abstract class AbstractAttackAlgorithm : IAttackStrategy
    {
        protected readonly Attack[] attacks;

        protected AbstractAttackAlgorithm(Attack[] attacks)
        {
            this.attacks = attacks;
        }

        public abstract Attack GetAttack();
    }
}