using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Contexts;
using rpsls.Domain.Algorithms.Modifiers;
using rpsls.Domain.Algorithms.RecencyBiases;
using rpsls.Domain.Modules;
using rpsls.Entities.Enums;

namespace rpsls.Application;

public interface IAttackService
{
    AttackTypes PredictNextAttack();
}

public class AttackService(
    IMatchModule matchModule,
    IRuleModule ruleModule,
    IModifierAlgorithm modifierAlgorithm,
    IRecencyBiasAlgorithm recencyBiasAlgorithm,
    ILogger<AttackService> logger) : IAttackService
{
    public AttackTypes PredictNextAttack()
    {
        var matchHistory = matchModule.GetAll();

        logger.LogTrace("Total Count: {TotalCount}", matchHistory.Count);

        if (!matchHistory.Any())
        {
            return (AttackTypes)Random.Shared.Next(1, 4);
        }

        var context = new AlgorithmContext
        {
            LastMatch = matchHistory[matchHistory.Count - 1],
            MatchHistory = matchHistory,
            TotalCount = matchHistory.Count
        };

        var attackPercentages = modifierAlgorithm.CalculatePercentages(matchHistory, context);
        attackPercentages = recencyBiasAlgorithm.ApplyRecencyBias(attackPercentages, context);

        var predictedNextAttack = attackPercentages
            .OrderByDescending(kv => kv.Value)
            .First();

        foreach (var attackPercentage in attackPercentages)
        {
            logger.LogDebug("{Attack}: {Percentage}", attackPercentage.Key, attackPercentage.Value);
        }

        logger.LogDebug("Predicted next attack: {PredictedNextAttack}", predictedNextAttack);

        return ruleModule.GetAttackToBeat(predictedNextAttack.Key);
    }
}