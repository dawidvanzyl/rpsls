using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.RecencyBiases;
using rpsls.Domain.Algorithms.Weighted;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;

namespace rpsls.Application;

public interface IAttackPredictor
{
    AttackTypes PredictNextAttack();
}

public class AttackPredictor(
    IGameModule gameModule,
    IRuleModule ruleModule,
    IWeightingAlgorithm weightedAlgorithm,
    IRecencyBiasAlgorithm recencyBiasAlgorithm,
    ILogger<AttackPredictor> logger) : IAttackPredictor
{
    public AttackTypes PredictNextAttack()
    {
        var gameHistory = gameModule.GetFullHistory();

        logger.LogTrace("Total Count: {TotalCount}", gameHistory.Count);

        if (!gameHistory.Any())
        {
            return (AttackTypes)Random.Shared.Next(1, 4);
        }

        var attackWeights = weightedAlgorithm.CalculateWeights(gameHistory);
        attackWeights = recencyBiasAlgorithm.ApplyRecencyBias(attackWeights);

        var predictedNextAttack = attackWeights
            .OrderByDescending(kv => kv.Value)
            .First();

        foreach (var attackPercentage in attackWeights)
        {
            logger.LogDebug("{Attack}: {Weight}", attackPercentage.Key, attackPercentage.Value);
        }

        logger.LogDebug("Predicted next attack: {PredictedNextAttack}", predictedNextAttack);

        return ruleModule.GetAttackToBeat(predictedNextAttack.Key);
    }
}