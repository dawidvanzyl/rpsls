using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using rpsls.Domain.Algorithms.Modifiers;
using rpsls.Domain.Algorithms.Weighted;
using rpsls.Entities;
using rpsls.Entities.Enums;
using Shouldly;
using Xunit.Abstractions;

namespace rpsls.Domain.Tests;

public class WeightedAlgorithmTests
{
    private readonly IList<IWeightingAlgorithm> _algorithms;
    private readonly ITestOutputHelper _output;

    public WeightedAlgorithmTests(ITestOutputHelper output)
    {
        var services = new ServiceCollection();

        services
            .AddKeyedSingleton<IWeightingAlgorithm, DynamicThresholdWeighting>("WeightedAlgorithm")
            .AddKeyedSingleton<IWeightingAlgorithm, ExponentialSmoothing>("WeightedAlgorithm")
            .AddKeyedSingleton<IWeightingAlgorithm, ExtremeRepeatsWeighting>("WeightedAlgorithm")
            .AddKeyedSingleton<IWeightingAlgorithm, FrequencyWeighting>("WeightedAlgorithm")
            .AddKeyedSingleton<IWeightingAlgorithm, LogarithmicWeighting>("WeightedAlgorithm")
            .AddKeyedSingleton<IWeightingAlgorithm, PercentileWeighting>("WeightedAlgorithm");

        services
            .AddSingleton<ILogger<DynamicThresholdWeighting>>(new NullLogger<DynamicThresholdWeighting>())
            .AddSingleton<ILogger<ExponentialSmoothing>>(new NullLogger<ExponentialSmoothing>())
            .AddSingleton<ILogger<ExtremeRepeatsWeighting>>(new NullLogger<ExtremeRepeatsWeighting>())
            .AddSingleton<ILogger<FrequencyWeighting>>(new NullLogger<FrequencyWeighting>())
            .AddSingleton<ILogger<LogarithmicWeighting>>(new NullLogger<LogarithmicWeighting>())
            .AddSingleton<ILogger<PercentileWeighting>>(new NullLogger<PercentileWeighting>());

        var serviceProvider = services.BuildServiceProvider();

        _algorithms = serviceProvider
            .GetKeyedServices<IWeightingAlgorithm>("WeightedAlgorithm")
            .ToList();

        _output = output;
    }

    [Fact]
    public void TestAlgorithmEffectiveness()
    {
        var historySize = 100;
        var history = GenerateGameHistory(historySize);
        var expectedWeights = GetExpectedWeights(history);

        foreach (var algorithm in _algorithms)
        {
            var calculatedWeights = algorithm.CalculateWeights(history);
            var algorithmName = algorithm.GetType().Name;
            AssertCalculatedWeights(algorithmName, calculatedWeights, expectedWeights);
            _output.WriteLine("");
        }
    }

    private static Round CreateRound((AttackTypes p1, AttackTypes p2) attacks, int repeat, int round)
    {
        var resultType = attacks switch
        {
            var (a, b) when a == b => ResultTypes.Draw,
            var (a, b) when
                (a == AttackTypes.Rock && b == AttackTypes.Scissor)
                || (a == AttackTypes.Paper && b == AttackTypes.Rock)
                || (a == AttackTypes.Scissor && b == AttackTypes.Paper) => ResultTypes.Win,
            _ => ResultTypes.Loss
        };

        return new Round(round, attacks.p1, attacks.p2, resultType, repeat);
    }

    private static IList<Round> GenerateGameHistory(int historySize)
    {
        var random = new Random();
        var history = new List<Round>();
        for (var i = 0; i < historySize; i++)
        {
            var p1Attack = (AttackTypes)random.Next(1, 4);
            var p2Attack = (AttackTypes)random.Next(1, 4);
            history.Add(CreateRound((p1Attack, p2Attack), GetConsecutiveRepeats(p1Attack, history), i + 1));
        }

        return history;
    }

    private static int GetConsecutiveRepeats(AttackTypes p1, IList<Round> history)
    {
        var consecutiveRepeat = 1;
        if (history.Count <= 0)
        {
            return consecutiveRepeat;
        }

        var repeated = history[^1].P1Attack == p1;
        if (repeated)
        {
            consecutiveRepeat = history[^1].ConsecutiveRepeats + 1;
        }

        return consecutiveRepeat;
    }

    private void AssertCalculatedWeights(string name, IDictionary<AttackTypes, decimal> calculatedWeights, IDictionary<AttackTypes, decimal> expectedWeights)
    {
        var sumOfWeights = calculatedWeights.Sum(kv => kv.Value);

        // Compare calculated weights to expected weights
        foreach (var attack in expectedWeights.Keys)
        {
            var expected = expectedWeights[attack];
            var calculated = calculatedWeights[attack];

            var actual = calculated / sumOfWeights;
            _output.WriteLine($"{name} (Attack: {attack}, Expected: {expected:F2}, Actual: {actual:F2}, Calculated: {calculated:F2})");
            actual.ShouldBeInRange(expected - 0.10m, expected + 0.10m, $"[{name}] {attack} - {actual}");
        }
    }

    private IDictionary<AttackTypes, decimal> GetExpectedWeights(IList<Round> history)
    {
        var attackPercentages = history
            .GroupBy((matchResult) => matchResult.P1Attack)
            .Select(attackGroup =>
            {
                return new
                {
                    Attack = attackGroup.Key,
                    Weight = (decimal)attackGroup.Count() / history.Count()
                };
            })
            .ToDictionary(
                a => a.Attack,
                a => a.Weight);

        return attackPercentages;
    }
}