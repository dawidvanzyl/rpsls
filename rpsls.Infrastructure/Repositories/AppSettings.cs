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

        IList<ChallengeValue> GetChallenges();

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

        public IList<ChallengeValue> GetChallenges()
        {
            var attackTypes = GetAttackValues();
            var attackNames = attackTypes.Select(attackValue => attackValue.Name);

            var challengeConfigs = configuration
                .GetSection("Challenges")
                .GetChildren();

            var allChallengeAttackNames = challengeConfigs.All(challengeConfig => attackNames.Contains(challengeConfig.Key));
            if (!allChallengeAttackNames)
            {
                throw new InvalidDataException("Challenges configuration contains invalid attack values");
            }

            var challenges = new List<ChallengeValue>();
            foreach (var challengeConfig in challengeConfigs)
            {
                var defeatedByConfigs = challengeConfig
                    .GetSection("DefeatedBy")
                    .GetChildren();

                var allDefeatedByAttackNames = defeatedByConfigs.All(defeatedByConfig => attackNames.Contains(defeatedByConfig.Key));
                if (!allDefeatedByAttackNames)
                {
                    throw new InvalidDataException($"DefeatedBy configuration for [{challengeConfig.Key}] contains invalid attack values");
                }

                var attack = attackTypes.First(attackValue => attackValue.Name.Equals(challengeConfig.Key));
                var defeatedByValues = defeatedByConfigs.Select(defeatedByConfig =>
                {
                    var defeatedBy = attackTypes.First(attackValue => attackValue.Name.Equals(defeatedByConfig.Key));
                    return DefeatedByValue.From(defeatedBy, defeatedByConfig.Value);
                });

                challenges.Add(ChallengeValue.From(attack, defeatedByValues.ToList()));
            }

            return challenges
                .OrderBy(challenge => challenge.Attack.Value)
                .ToList();
        }

        public IList<GameValue> GetGames()
        {
            var challenges = GetChallenges();
            var gameConfigurations = configuration
                .GetSection("Game")
                .GetChildren();

            var gameValues = new List<GameValue>();
            foreach (var gameConfiguration in gameConfigurations)
            {
                var lastChallengeIndex = Convert.ToByte(gameConfiguration.Value);
                var gameChallenges = challenges.Where(challenge => challenge.Attack.Value <= lastChallengeIndex);

                var challengeIndexValid = gameChallenges
                    .Count()
                    .Equals(lastChallengeIndex);

                if (!challengeIndexValid)
                {
                    throw new ArgumentOutOfRangeException(gameConfiguration.Key, "Configuration cannot be transformed into challenges.");
                }

                gameValues.Add(GameValue.From(gameConfiguration.Key, gameChallenges.ToList()));
            }

            return gameValues;
        }
    }
}