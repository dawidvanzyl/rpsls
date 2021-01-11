using Microsoft.Extensions.Configuration;
using rpsls.Domain;
using rpsls.Domain.Values;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace rpsls.Infrastructure
{
    public class AppSettings
    {
        private readonly IConfiguration configuration;

        public AppSettings(IConfigurationFactory configurationFactory)
        {
            configuration = configurationFactory.GetConfiguration();
        }

        public IList<AttackValue> GetAttackValues()
        {
            var attackTypeConfigs = configuration.GetSection("AttackValues")
                .GetChildren();

            var attackTypes = new List<AttackValue>();
            foreach (var attackTypeConfig in attackTypeConfigs)
            {
                attackTypes.Add(AttackValue.From(attackTypeConfig.Key, Convert.ToByte(attackTypeConfig.Value)));
            }

            return attackTypes
                .OrderBy(attackType => attackType.Value)
                .ToList();
        }

        public IList<Attack> GetAttacks()
        {
            var attackValues = GetAttackValues();
            var attackNames = attackValues.Select(attackValue => attackValue.Name);

            var attackConfigs = configuration.GetSection("Attacks")
                .GetChildren();

            var allAttackValuesValid = attackConfigs.All(attackConfig => attackNames.Contains(attackConfig.Key));
            if (!allAttackValuesValid)
            {
                throw new InvalidDataException("Attacks configuration contains invalid attack values");
            }

            var attacks = new List<Attack>();
            foreach (var attackConfig in attackConfigs)
            {
                var defeatedByConfigs = attackConfig.GetSection("DefeatedBy")
                    .GetChildren();

                var allDefeatedByAttackValuesValid = defeatedByConfigs.All(defeatedByConfig => attackNames.Contains(defeatedByConfig.Key));
                if (!allDefeatedByAttackValuesValid)
                {
                    throw new InvalidDataException($"DefeatedBy configuration for [{attackConfig.Key}] contains invalid attack values");
                }

                var attackValue = attackValues.First(attackValue => attackValue.Name.Equals(attackConfig.Key));
                var defeatedByValues = defeatedByConfigs.Select(defeatedByConfig =>
                {
                    var attackValue = attackValues.First(attackValue => attackValue.Name.Equals(defeatedByConfig.Key));
                    return DefeatedByValue.From(attackValue, defeatedByConfig.Value);
                });

                attacks.Add(Attack.From(attackValue, defeatedByValues.ToList()));
            }

            return attacks;
        }

        public GameValue GetGameValue()
        {
            var gameConfiguration = configuration["Game:Configuration"];
            var attackConfigurations = configuration.GetSection("Game").GetSection("Attacks")
                .GetChildren();

            var attackConfigurationValid = attackConfigurations.Any(attackConfiguration => attackConfiguration.Key.Equals(gameConfiguration, StringComparison.OrdinalIgnoreCase));
            if (!attackConfigurationValid)
            {
                throw new ArgumentOutOfRangeException(gameConfiguration, $"Game configuration does not have an attack configuration.");
            }

            var attacks = GetAttacks();
            var lastAttackIndex = Convert.ToByte(configuration[$"Game:Attacks:{gameConfiguration}"]);

            var gameAttacks = attacks.Where(attack => attack.AttackValue.Value <= lastAttackIndex);
            var attackIndexValid = gameAttacks.Count().Equals(lastAttackIndex);

            if (!attackIndexValid)
            {
                throw new ArgumentOutOfRangeException(gameConfiguration, "Attack configuration cannot be transformed into attacks.");
            }

            var rounds = Convert.ToInt16(configuration["Game:Rounds"]);

            return GameValue.From(gameConfiguration, gameAttacks.ToList(), rounds);
        }
    }
}
