using rpsls.Domain.Enums;
using rpsls.Domain.Values;

namespace rpsls.Domain
{
    public class PlayerResult
    {
        public AttackResult AttackResult { get; set; }

        public string Id { get; set; }

        public AttackValue PlayerAttack { get; set; }
    }
}