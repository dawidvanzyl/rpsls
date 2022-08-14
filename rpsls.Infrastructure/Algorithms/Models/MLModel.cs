using Microsoft.ML.Data;

namespace rpsls.Infrastructure.Algorithms.Models
{
    public class MLModel
    {
        [KeyType(count: 262111)]
        public uint AI { get; set; }

        public float Label { get; set; }

        [KeyType(count: 262111)]
        public uint Player { get; set; }
    }
}