using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Attacks
{
    public interface IAttackFactory
    {
        IList<Attack> GetAttacks();
    }

    public sealed class AttackFactory : IAttackFactory
    {
        private readonly IConfiguration configuration;

        public AttackFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IList<Attack> GetAttacks()
        {
            var attackTypes = CreateAttackTypes();

            var attacks = new List<Attack>();
            foreach (var attackType in attackTypes)
            {
                var defeatedByConfigs = configuration.GetSection(attackType.Name)
                    .GetSection("DefeatedBy")
                    .GetChildren();

                var defeatConfigs = new List<DefeatConfig>();
                foreach (var defeatedByConfig in defeatedByConfigs)
                {
                    if (!attackTypes.Any(attackType => attackType.Name.Equals(defeatedByConfig.Key, StringComparison.OrdinalIgnoreCase)))
                    {
                        throw new ArgumentOutOfRangeException(defeatedByConfig.Key, "DefeatedBy Attack enum not found in the Attacks configuration");
                    }

                    var defeatedByAttackType = attackTypes.First(attackType => attackType.Name.Equals(defeatedByConfig.Key, StringComparison.OrdinalIgnoreCase));
                    defeatConfigs.Add(DefeatConfig.From(defeatedByAttackType, defeatedByConfig.Value));
                }

                attacks.Add(Attack.From(attackType, defeatConfigs));
            }

            return attacks
                .OrderBy(attack => attack.AttackType.Value)
                .ToList();
        }

        private IList<AttackType> CreateAttackTypes()
        {
            var attackTypeConfigs = configuration.GetSection("Attacks")
                .GetChildren();

            var attackTypes = new List<AttackType>();
            foreach (var attackTypeConfig in attackTypeConfigs)
            {
                attackTypes.Add(AttackType.From(attackTypeConfig.Key, Convert.ToByte(attackTypeConfig.Value)));
            }

            return attackTypes;
        }
    }
}
