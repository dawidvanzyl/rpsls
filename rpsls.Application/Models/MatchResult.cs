using rpsls.Domain.Enums;

namespace rpsls.Application.Models
{
    public class MatchResult
    {
        public PlayerInput PlayerOne { get; internal set; }

        public PlayerInput PlayerTwo { get; internal set; }

        public byte Rounds { get; internal set; }

        public string GetMatchMessage()
        {
            return (ChallengeResult)PlayerOne.Won.CompareTo(PlayerTwo.Won) switch
            {
                ChallengeResult.Won => $"{PlayerOne.Name} won {PlayerOne.Won} to {PlayerTwo.Won}.",
                ChallengeResult.Tied => $"{PlayerOne.Name} and {PlayerTwo.Name} tied.",
                ChallengeResult.Lost => $"{PlayerTwo.Name} won {PlayerTwo.Won} to {PlayerOne.Won}."
            };
        }

        internal void Win(PlayerInput player)
        {
            player.Won++;
        }
    }
}