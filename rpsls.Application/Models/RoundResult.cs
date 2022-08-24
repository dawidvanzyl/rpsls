using rpsls.Domain.Enums;
using rpsls.Domain.Values;

namespace rpsls.Application.Models
{
    public class RoundResult
    {
        public string ChallengeMessage { get; internal set; }

        public ChallengeResult ChallengeResult { get; internal set; }

        public string DefeatMessage { get; internal set; }

        public ChallengeValue PlayerOne { get; internal set; }

        public ChallengeValue PlayerTwo { get; internal set; }
    }
}