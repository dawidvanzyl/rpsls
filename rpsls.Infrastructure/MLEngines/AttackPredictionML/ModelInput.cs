using Microsoft.ML.Data;

namespace rpsls.Infrastructure.MLEngines.AttackPredictionML
{
    public class ModelInput
    {
        [ColumnName("computer"), LoadColumn(0)]
        public string Computer { get; set; }

        [ColumnName("player"), LoadColumn(1)]
        public float Player { get; set; }

        [ColumnName("result"), LoadColumn(2)]
        public float Result { get; set; }
    }
}
