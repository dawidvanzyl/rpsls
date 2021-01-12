using rpsls.Domain;
using rpsls.Domain.Enums;

namespace rpsls.Application.Entities
{
    public sealed class MatchResult
    {
        private MatchResult(Attack player1, Attack player2, AttackResult attackResult)
        {
            Player1 = player1;
            Player2 = player2;
            AttackResult = attackResult;
        }

        public Attack Player1 { get; }
        public Attack Player2 { get; }
        public AttackResult AttackResult { get; }

        public static MatchResult From(Attack player1, Attack player2, AttackResult attackResult)
        {
            return new MatchResult(player1, player2, attackResult);
        }
    }
}
