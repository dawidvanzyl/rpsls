using System;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Attacks
{
    public class Attack
    {
        private readonly IList<DefeatConfig> defeatConfigs;

        private Attack(AttackType attackType, IList<DefeatConfig> defeatConfigs)
        {
            AttackType = attackType;
            this.defeatConfigs = defeatConfigs;
        }

        public AttackType AttackType { get; }

        public AttackResult GetAttackResult(AttackType attackType)
        {
            if (attackType.Equals(AttackType))
            {
                return AttackResult.Tie;
            }

            if (defeatConfigs.Any(defeatConfig => defeatConfig.AttackType.Equals(attackType))) 
            {
                return AttackResult.Lose;
            }

            return AttackResult.Win;
        }

        public string GetDefeatMessage(AttackType attackType)
        { 
            if (!defeatConfigs.Any(defeatConfig => defeatConfig.AttackType.Equals(attackType)))
            {
                throw new ArgumentOutOfRangeException(attackType.Name, "DefeatedBy Attack enum not found in the defeat configs");
            }

            return defeatConfigs.First(defeatConfig => defeatConfig.AttackType.Equals(attackType))
                .Message;
        }

        public static Attack From(AttackType attackType, IList<DefeatConfig> defeatConfigs)
        {
            return new Attack(attackType, defeatConfigs);
        }
    }
}
