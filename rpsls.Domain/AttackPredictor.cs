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
    IMatchModule matchModule,
    IRuleModule ruleModule,
    IWeightingAlgorithm weightedAlgorithm,
    IRecencyBiasAlgorithm recencyBiasAlgorithm,
    ILogger<AttackPredictor> logger) : IAttackPredictor
{
    public AttackTypes PredictNextAttack()
    {
        var matchHistory = matchModule.GetAll();

        logger.LogTrace("Total Count: {TotalCount}", matchHistory.Count);

        if (!matchHistory.Any())
        {
            return (AttackTypes)Random.Shared.Next(1, 4);
        }

        var attackWeights = weightedAlgorithm.CalculateWeights(matchHistory);

        if (!matchHistory[^1].IsNew)
        {
            attackWeights = recencyBiasAlgorithm.ApplyRecencyBias(attackWeights, matchHistory);
        }

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