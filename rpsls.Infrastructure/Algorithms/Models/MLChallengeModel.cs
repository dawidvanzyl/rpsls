using Microsoft.ML.Data;
using rpsls.Domain;
using rpsls.Domain.Values;

namespace rpsls.Infrastructure.Algorithms.Models
{
    public class MLChallengeModel
    {
        [KeyType(count: 262111)]
        public uint AI { get; set; }

        public string GameName { get; set; }

        [KeyType(count: 262111)]
        public uint Player { get; set; }

        public float Score { get; set; }

        public static MLChallengeModel From(string gameName, GameRound gameRound)
        {
            return new MLChallengeModel { GameName = gameName, Player = gameRound.PlayerOne.AttackValue, AI = gameRound.PlayerTwo.AttackValue };
        }

        public static MLChallengeModel From(string gameName, ChallengeValue player, ChallengeValue ai)
        {
            return new MLChallengeModel { GameName = gameName, Player = player.Attack.Value, AI = ai.Attack.Value };
        }
    }
}