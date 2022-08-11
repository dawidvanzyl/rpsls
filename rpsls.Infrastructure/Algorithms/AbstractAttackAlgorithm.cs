using rpsls.Domain;

namespace rpsls.Infrastructure.Algorithms
{
    public interface IAttackStrategy
    {
        Attack GetAttack();
    }

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