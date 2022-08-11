using Microsoft.Extensions.Configuration;
using rpsls.Domain;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Factories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace rpsls.Infrastructure.Repositories
{
    public interface ISettingsRepository
    {
        IList<Attack> GetAttacks();

        IList<AttackValue> GetAttackValues();

        IList<GameValue> GetGameValues();
    }

    public class AppSettings : ISettingsRepository
    {
        private readonly IConfiguration configuration;

        public AppSettings(IConfigurationFactory configurationFactory)
        {
            configuration = configurationFactory.GetConfiguration();
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

            return attacks
                .OrderBy(attack => attack.AttackValue.Value)
                .ToList();
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

        public IList<GameValue> GetGameValues()
        {
            var attacks = GetAttacks();
            var gameConfigurations = configuration.GetSection("Game")
                .GetChildren();

            var gameValues = new List<GameValue>();
            foreach (var gameConfiguration in gameConfigurations)
            {
                var lastAttackIndex = Convert.ToByte(gameConfiguration.Value);
                var gameAttacks = attacks.Where(attack => attack.AttackValue.Value <= lastAttackIndex);

                var attackIndexValid = gameAttacks.Count().Equals(lastAttackIndex);
                if (!attackIndexValid)
                {
                    throw new ArgumentOutOfRangeException(gameConfiguration.Key, "Configuration cannot be transformed into attacks.");
                }

                gameValues.Add(GameValue.From(gameConfiguration.Key, gameAttacks.ToList()));
            }

            return gameValues;
        }
    }
}