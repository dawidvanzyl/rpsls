using Microsoft.Extensions.Configuration;
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
        IList<AttackValue> GetAttackValues();

        IList<ChallangeValue> GetChallanges();

        IList<GameValue> GetGames();
    }

    public class AppSettings : ISettingsRepository
    {
        private readonly IConfiguration configuration;

        public AppSettings(IConfigurationFactory configurationFactory)
        {
            configuration = configurationFactory.GetConfiguration();
        }

        public IList<AttackValue> GetAttackValues()
        {
            var attackTypeConfigs = configuration
                .GetSection("AttackValues")
                .GetChildren();

            var attackTypes = new List<AttackValue>();
            foreach (var attackTypeConfig in attackTypeConfigs)
            {
                attackTypes.Add(AttackValue.From(attackTypeConfig.Key, Convert.ToByte(attackTypeConfig.Value)));
            }

            return attackTypes
                .OrderBy(attackValue => attackValue.Value)
                .ToList();
        }

        public IList<ChallangeValue> GetChallanges()
        {
            var attackTypes = GetAttackValues();
            var attackNames = attackTypes.Select(attackValue => attackValue.Name);

            var challangeConfigs = configuration
                .GetSection("Challanges")
                .GetChildren();

            var allChallangeAttackNames = challangeConfigs.All(challangeConfig => attackNames.Contains(challangeConfig.Key));
            if (!allChallangeAttackNames)
            {
                throw new InvalidDataException("Challanges configuration contains invalid attack values");
            }

            var challanges = new List<ChallangeValue>();
            foreach (var challangeConfig in challangeConfigs)
            {
                var defeatedByConfigs = challangeConfig
                    .GetSection("DefeatedBy")
                    .GetChildren();

                var allDefeatedByAttackNames = defeatedByConfigs.All(defeatedByConfig => attackNames.Contains(defeatedByConfig.Key));
                if (!allDefeatedByAttackNames)
                {
                    throw new InvalidDataException($"DefeatedBy configuration for [{challangeConfig.Key}] contains invalid attack values");
                }

                var attack = attackTypes.First(attackValue => attackValue.Name.Equals(challangeConfig.Key));
                var defeatedByValues = defeatedByConfigs.Select(defeatedByConfig =>
                {
                    var defeatedBy = attackTypes.First(attackValue => attackValue.Name.Equals(defeatedByConfig.Key));
                    return DefeatedByValue.From(defeatedBy, defeatedByConfig.Value);
                });

                challanges.Add(ChallangeValue.From(attack, defeatedByValues.ToList()));
            }

            return challanges
                .OrderBy(challange => challange.Attack.Value)
                .ToList();
        }

        public IList<GameValue> GetGames()
        {
            var challanges = GetChallanges();
            var gameConfigurations = configuration
                .GetSection("Game")
                .GetChildren();

            var gameValues = new List<GameValue>();
            foreach (var gameConfiguration in gameConfigurations)
            {
                var lastChallangeIndex = Convert.ToByte(gameConfiguration.Value);
                var gameChallanges = challanges.Where(challange => challange.Attack.Value <= lastChallangeIndex);

                var challangeIndexValid = gameChallanges
                    .Count()
                    .Equals(lastChallangeIndex);

                if (!challangeIndexValid)
                {
                    throw new ArgumentOutOfRangeException(gameConfiguration.Key, "Configuration cannot be transformed into challanges.");
                }

                gameValues.Add(GameValue.From(gameConfiguration.Key, gameChallanges.ToList()));
            }

            return gameValues;
        }
    }
}