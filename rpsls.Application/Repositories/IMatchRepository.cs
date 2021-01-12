using rpsls.Domain;
using rpsls.Domain.Enums;

namespace rpsls.Application.Repositories
{
    public interface IMatchRepository
    {
        void Add(Attack playerReaction, AttackResult previousPlayer1AttackResult, Attack previousPlayer2Attack);
    }
}
