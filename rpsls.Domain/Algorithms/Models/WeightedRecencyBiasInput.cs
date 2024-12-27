using rpsls.Entities.Enums;

namespace rpsls.Domain.Algorithms.Models;

public class WeightedRecencyBiasInput
{
    public ResultTypes LastResult { get; init; }

    public decimal WeighedPercentage { get; init; }
}