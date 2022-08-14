using Microsoft.ML.Data;
using rpsls.Domain;
using rpsls.Domain.Values;
using System;

namespace rpsls.Infrastructure.Algorithms.Models
{
    public class MLPlayerAttackModel
    {
        [KeyType(count: 262111)]
        public uint Attack { get; set; }

        [KeyType(count: 262111)]
        public uint ChallengeResult { get; set; }

        public string GameName { get; set; }
        public float Score { get; set; }

        public static MLPlayerAttackModel From(string gameName, ChallengeValue player, ChallengeValue ai)
        {
            return new MLPlayerAttackModel { GameName = gameName, Attack = player.Attack.Value, ChallengeResult = Convert.ToUInt32(player.CompareTo(ai)) };
        }

        public static MLPlayerAttackModel From(string gameName, Challenge player)
        {
            return new MLPlayerAttackModel { GameName = gameName, Attack = player.AttackValue, ChallengeResult = Convert.ToUInt32(player.ChallengeResult) };
        }
    }
}