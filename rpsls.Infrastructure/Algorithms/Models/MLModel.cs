using Microsoft.ML.Data;
using rpsls.Domain;
using rpsls.Domain.Values;

namespace rpsls.Infrastructure.Algorithms.Models
{
    public class MLModel
    {
        [KeyType(count: 262111)]
        public uint AI { get; set; }

        public int ChallangeResult { get; set; }

        public string GameName { get; set; }

        [KeyType(count: 262111)]
        public uint Player { get; set; }

        public float Score { get; set; }

        public static MLModel From(string gameName, GameRound gameRound)
        {
            return new MLModel { GameName = gameName, Player = gameRound.PlayerOne.AttackValue, AI = gameRound.PlayerTwo.AttackValue, ChallangeResult = (int)gameRound.PlayerOne.ChallengeResult };
        }

        public static MLModel From(string gameName, ChallengeValue player, ChallengeValue ai)
        {
            return new MLModel { GameName = gameName, Player = player.Attack.Value, AI = ai.Attack.Value, ChallangeResult = player.CompareTo(ai) };
        }
    }
}