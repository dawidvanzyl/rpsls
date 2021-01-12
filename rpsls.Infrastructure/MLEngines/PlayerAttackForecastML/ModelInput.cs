using Microsoft.ML.Data;

namespace rpsls.Infrastructure.MLEngines.PlayerAttackForecastML
{
    public class ModelInput
    {
        [ColumnName("player_reaction"), LoadColumn(0)]
        public string Player_reaction { get; set; }

        [ColumnName("previous_player_result"), LoadColumn(1)]
        public float Previous_player_result { get; set; }

        [ColumnName("previous_computer"), LoadColumn(2)]
        public float Previous_computer { get; set; }
    }
}
