using rpsls.Game;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Attacks
{
    public class Attack : IComparable<Attack>
    {
        private readonly IList<DefeatConfig> defeatConfigs;

        private Attack(AttackType attackType, IList<DefeatConfig> defeatConfigs)
        {
            AttackType = attackType;
            this.defeatConfigs = defeatConfigs;
        }

        public AttackType AttackType { get; }

        public string GetDefeatMessage(AttackType attackType)
        { 
            if (!defeatConfigs.Any(defeatConfig => defeatConfig.AttackType.Equals(attackType)))
            {
                throw new ArgumentOutOfRangeException(attackType.Name, "DefeatedBy Attack enum not found in the defeat configs");
            }

            return defeatConfigs.First(defeatConfig => defeatConfig.AttackType.Equals(attackType))
                .Message;
        }

        public int CompareTo(Attack other)
        {
            if (AttackType.Equals(other.AttackType))
            {
                return (int)AttackResult.Tie;
            }

            if (defeatConfigs.Any(defeatConfig => defeatConfig.AttackType.Equals(other.AttackType)))
            {
                return (int)AttackResult.Lose;
            }

            return (int)AttackResult.Win;
        }

        public static Attack From(AttackType attackType, IList<DefeatConfig> defeatConfigs)
        {
            return new Attack(attackType, defeatConfigs);
        }
    }
}
