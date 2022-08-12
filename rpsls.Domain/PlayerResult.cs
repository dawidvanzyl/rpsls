using rpsls.Domain.Enums;
using rpsls.Domain.Values;

namespace rpsls.Domain
{
    public class PlayerResult
    {
        public ChallangeResult ChallangeResult { get; set; }

        public string Id { get; set; }

        public AttackValue PlayerAttack { get; set; }
    }
}