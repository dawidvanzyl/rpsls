using rpsls.Domain.Algorithms.Modifiers;
using rpsls.Domain.Algorithms.RecencyBiases;

namespace rpsls.IoC.Options
{
    public class AlgorithmOptions
    {
        public string Modifier { get; set; } = nameof(ExponentialSmoothing); // Default

        public string RecencyBias { get; set; } = nameof(WeightedRecencyBias); // Default
    }
}