using Microsoft.ML.Data;
using rpsls.Domain;
using rpsls.Domain.Values;

namespace rpsls.Infrastructure.Algorithms.Models
{
    public class MLModel
    {
        [KeyType(count: 262111)]
        public uint ChallangeResult { get; set; }

        public string GameName { get; set; }

        [KeyType(count: 262111)]
        public uint Machine { get; set; }

        [KeyType(count: 262111)]
        public uint Player { get; set; }

        public float Score { get; set; }

        public static MLModel From(string gameName, GameRound gameRound)
        {
            return new MLModel { GameName = gameName, Player = gameRound.PlayerOne.AttackValue, Machine = gameRound.PlayerTwo.AttackValue, ChallangeResult = (uint)gameRound.PlayerOne.ChallengeResult + 1 };
        }

        public static MLModel From(string gameName, ChallengeValue player, ChallengeValue machine)
        {
            return new MLModel { GameName = gameName, Player = player.Attack.Value, Machine = machine.Attack.Value, ChallangeResult = (uint)player.CompareTo(machine) + 1 };
        }

        public MLModel ReverseChallanges(int challangeAdjust)
        {
            return new MLModel
            {
                ChallangeResult = (uint)((int)ChallangeResult + challangeAdjust),
                GameName = GameName,
                Machine = Player,
                Player = Machine,
                Score = Score
            };
        }
    }
}