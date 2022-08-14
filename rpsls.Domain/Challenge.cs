using rpsls.Domain.Enums;

namespace rpsls.Domain
{
    public class Challenge
    {
        public byte AttackValue { get; set; }

        public ChallengeResult ChallengeResult { get; set; }

        public string Id { get; set; }
    }
}