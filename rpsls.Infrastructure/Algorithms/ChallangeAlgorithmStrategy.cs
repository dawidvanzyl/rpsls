using rpsls.Domain.Values;
using rpsls.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public interface IChallangeAlgorithmStrategy
    {
        IChallangeAlgorithm CreateMLChallange(GameValue gameValue);

        IChallangeAlgorithm CreateRandomChallange(GameValue gameValue);
    }

    public class ChallangeAlgorithmStrategy : IChallangeAlgorithmStrategy
    {
        private readonly IDictionary<string, IChallangeAlgorithm> challangeStrategies;

        public ChallangeAlgorithmStrategy(ISettingsRepository settingsRepository)
        {
            var gameValues = settingsRepository.GetGames();

            challangeStrategies = new Dictionary<string, IChallangeAlgorithm>();
            foreach (var gameValue in gameValues)
            {
                challangeStrategies.Add($"{gameValue.Name}_{nameof(RandomChallangeAlgorithm)}", new RandomChallangeAlgorithm(gameValue.Challanges.ToArray()));
                challangeStrategies.Add($"{gameValue.Name}_{nameof(MLChallangeAlgorithm)}", new MLChallangeAlgorithm(gameValue.Challanges.ToArray()));
            }
        }

        public IChallangeAlgorithm CreateMLChallange(GameValue gameValue)
        {
            return GetAlgorithm($"{gameValue.Name}_{nameof(MLChallangeAlgorithm)}");
        }

        public IChallangeAlgorithm CreateRandomChallange(Domain.Values.GameValue gameValue)
        {
            return GetAlgorithm($"{gameValue.Name}_{nameof(RandomChallangeAlgorithm)}");
        }

        private IChallangeAlgorithm GetAlgorithm(string algorithmKey)
        {
            var gameServiceRegistered = challangeStrategies.ContainsKey(algorithmKey);
            if (!gameServiceRegistered)
            {
                throw new InvalidOperationException($"Challange algorithm {algorithmKey} has not been registered.");
            }

            var challangeStrategy = challangeStrategies[algorithmKey];
            return challangeStrategy;
        }
    }
}