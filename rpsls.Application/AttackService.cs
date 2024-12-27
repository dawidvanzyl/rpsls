using Microsoft.Extensions.Logging;
using rpsls.Domain.Algorithms.Models;
using rpsls.Domain.Algorithms.RecencyBias;
using rpsls.Domain.Algorithms.Weight;
using rpsls.Domain.Modules;
using rpsls.Entities;
using rpsls.Entities.Enums;

namespace rpsls.Application;

public interface IAttackService
{
    AttackTypes CalculateAttack();
}

public class AttackService(
    IMatchModule matchModule,
    IRuleModule ruleModule,
    IWeightAlgorithm<WeightedSmoothedCountInput> weightAlgorithm,
    IRecencyBiasAlgorithm<WeightedRecencyBiasInput> recencyBiasAlgorithm,
    ILogger<AttackService> logger) : IAttackService
{
    public AttackTypes CalculateAttack()
    {
        var matchHistory = matchModule.GetAll();

        if (!matchHistory.Any())
        {
            return (AttackTypes)Random.Shared.Next(1, 4);
        }

        var totalCount = matchHistory.Count;
        var lastMatch = matchHistory[matchHistory.Count - 1];

        var attackPercentages = matchHistory
            .GroupBy((matchResult) => matchResult.P1Attack)
            .Select(attackGroup =>
                new
                {
                    Attack = attackGroup.Key,
                    WeighedPercentage = CalculateWeightedPercentage(attackGroup, totalCount, lastMatch)
                })
            .OrderByDescending(a => a.WeighedPercentage)
            .ToList();

        var player1Prediction = attackPercentages[0].Attack;

        return ruleModule.GetAttackToBeat(player1Prediction);
    }

    private decimal CalculateWeightedPercentage(IGrouping<AttackTypes, Match> attackGroup, int totalCount, Match lastMatch)
    {
        var count = attackGroup.Count();

        logger.LogDebug("Attack Group: {AttackGroup}", attackGroup.Key);
        logger.LogDebug("Total Count: {TotalCount}", totalCount);
        logger.LogDebug("Count: {Count}", count);

        var weighedPercentage = weightAlgorithm.CalculateWeightedPercentage(
            new WeightedSmoothedCountInput
            {
                GroupCount = count,
                TotalCount = totalCount
            });

        if (lastMatch.P1Attack == attackGroup.Key)
        {
            weighedPercentage = recencyBiasAlgorithm.ApplyRecencyBias(
                new WeightedRecencyBiasInput
                {
                    LastResult = lastMatch.Result,
                    WeighedPercentage = weighedPercentage
                });
        }

        return weighedPercentage;
    }
}