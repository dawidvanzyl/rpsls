using Microsoft.Extensions.DependencyInjection;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;
using rpsls.Infrastructure.Repositories;
using System;
using System.Collections.Generic;

namespace rpsls.Infrastructure.Algorithms
{
    public interface IChallengeAlgorithmStrategy
    {
        MLChallengeAlgorithm CreateMLChallenge(GameValue gameValue);

        RandomChallengeAlgorithm CreateRandomChallenge(GameValue gameValue);
    }

    public class ChallengeAlgorithmStrategy : IChallengeAlgorithmStrategy
    {
        private readonly IDictionary<string, IChallengeAlgorithm> challengeStrategies;

        public ChallengeAlgorithmStrategy(ISettingsRepository settingsRepository, IServiceProvider serviceProvider)
        {
            var gameValues = settingsRepository.GetGames();

            challengeStrategies = new Dictionary<string, IChallengeAlgorithm>();
            foreach (var gameValue in gameValues)
            {
                SetupChallenges(gameValue, serviceProvider.GetRequiredService<RandomChallengeAlgorithm>());
                SetupChallenges(gameValue, serviceProvider.GetRequiredService<MLChallengeAlgorithm>());
            }

            void SetupChallenges(GameValue gameValue, IChallengeAlgorithm challengeAlgorithm)
            {
                challengeAlgorithm.SetupChallenges(gameValue);
                challengeStrategies.Add($"{gameValue.Name}_{challengeAlgorithm.GetType().Name}", challengeAlgorithm);
            }
        }

        public MLChallengeAlgorithm CreateMLChallenge(GameValue gameValue)
        {
            return GetAlgorithm<MLAlgorithmContext>($"{gameValue.Name}_{nameof(MLChallengeAlgorithm)}") as MLChallengeAlgorithm;
        }

        public RandomChallengeAlgorithm CreateRandomChallenge(GameValue gameValue)
        {
            return GetAlgorithm<AlgorithmContext>($"{gameValue.Name}_{nameof(RandomChallengeAlgorithm)}") as RandomChallengeAlgorithm;
        }

        private IChallengeAlgorithm<TAlgorithmContext> GetAlgorithm<TAlgorithmContext>(string algorithmKey)
            where TAlgorithmContext : AlgorithmContext
        {
            var gameServiceRegistered = challengeStrategies.ContainsKey(algorithmKey);
            if (!gameServiceRegistered)
            {
                throw new InvalidOperationException($"Challenge algorithm {algorithmKey} has not been registered.");
            }

            var challengeStrategy = challengeStrategies[algorithmKey];
            return challengeStrategy as IChallengeAlgorithm<TAlgorithmContext>;
        }
    }
}