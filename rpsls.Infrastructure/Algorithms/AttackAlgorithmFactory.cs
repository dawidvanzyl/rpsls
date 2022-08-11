using rpsls.Domain.Values;
using rpsls.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public interface IAttackStrategyFactory
    {
        IAttackStrategy CreateMLAttackAlgorithm(GameValue gameValue);

        IAttackStrategy CreateRandomAttackAlgorithm(GameValue gameValue);
    }

    public class AttackAlgorithmFactory : IAttackStrategyFactory
    {
        private readonly IDictionary<string, IAttackStrategy> attackStrategies;

        public AttackAlgorithmFactory(ISettingsRepository settingsRepository)
        {
            var gameValues = settingsRepository.GetGameValues();

            attackStrategies = new Dictionary<string, IAttackStrategy>();
            foreach (var gameValue in gameValues)
            {
                attackStrategies.Add($"{gameValue.Name}_{nameof(RandomAttackAlgorithm)}", new RandomAttackAlgorithm(gameValue.Attacks.ToArray()));
                attackStrategies.Add($"{gameValue.Name}_{nameof(MLAttackAlgorithm)}", new MLAttackAlgorithm(gameValue.Attacks.ToArray()));
            }
        }

        public IAttackStrategy CreateMLAttackAlgorithm(GameValue gameValue)
        {
            var randomAttackAlgorithmKey = $"{gameValue.Name}_{nameof(MLAttackAlgorithm)}";

            var gameServiceRegistered = attackStrategies.ContainsKey(randomAttackAlgorithmKey);
            if (!gameServiceRegistered)
            {
                throw new InvalidOperationException($"Game value {randomAttackAlgorithmKey} has not been registered.");
            }

            var attackStrategy = attackStrategies[randomAttackAlgorithmKey];
            return attackStrategy;
        }

        public IAttackStrategy CreateRandomAttackAlgorithm(Domain.Values.GameValue gameValue)
        {
            var randomAttackAlgorithmKey = $"{gameValue.Name}_{nameof(RandomAttackAlgorithm)}";

            var gameServiceRegistered = attackStrategies.ContainsKey(randomAttackAlgorithmKey);
            if (!gameServiceRegistered)
            {
                throw new InvalidOperationException($"Game value {randomAttackAlgorithmKey} has not been registered.");
            }

            var attackStrategy = attackStrategies[randomAttackAlgorithmKey];
            return attackStrategy;
        }
    }
}