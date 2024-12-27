namespace rpsls.Domain.Algorithms.RecencyBias;

public interface IRecencyBiasAlgorithm<in TInput>
    where TInput : class
{
    public decimal ApplyRecencyBias(TInput input);
}