using rpsls.Domain.Algorithms.Contexts;
using rpsls.Entities.Enums;
using System.Collections.Immutable;

namespace rpsls.Domain.Algorithms.RecencyBiases;

public interface IRecencyBiasAlgorithm
{
    public IImmutableDictionary<AttackTypes, decimal> ApplyRecencyBias(IImmutableDictionary<AttackTypes, decimal> attackPercentages, AlgorithmContext context);
}