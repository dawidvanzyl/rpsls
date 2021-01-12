using rpsls.Application.Entities;
using rpsls.Domain;

namespace rpsls.Application
{
    public interface IAttackStrategy
    {
        Attack GetAttack(MatchResult previousMatchResult);
    }
}
