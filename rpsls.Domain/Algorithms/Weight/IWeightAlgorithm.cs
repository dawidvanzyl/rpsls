namespace rpsls.Domain.Algorithms.Weight;

public interface IWeightAlgorithm<in TInput>
    where TInput : class
{
    decimal CalculateWeightedPercentage(TInput input);
}