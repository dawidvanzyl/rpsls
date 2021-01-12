using rpsls.Application.Entities;
using rpsls.Domain;
using rpsls.Domain.Enums;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public class MLAttackAlgorithm : AbstractAttackAlgorithm
    {
        public MLAttackAlgorithm(Attack[] attacks) 
            : base(attacks)
        {

        }

        public override Attack GetAttack(MatchResult previousMatchResult)
        {
            MLEngines.PlayerAttackForecastML.ModelBuilder.CreateModel();

            var playerAttackForecastModelInput = new MLEngines.PlayerAttackForecastML.ModelInput
            {
                Previous_player_result = (float)previousMatchResult.AttackResult,
                Previous_computer = previousMatchResult.Player2.AttackValue.Value
            };

            var predictedPlayerAttackResult = MLEngines.PlayerAttackForecastML.ConsumeModel.Predict(playerAttackForecastModelInput);

            var attackModelInput = new MLEngines.AttackPredictionML.ModelInput 
            {
                Player = float.Parse(predictedPlayerAttackResult.Prediction),
                Result = (float)AttackResult.Won
            };

            var predictedAttackResult = MLEngines.AttackPredictionML.ConsumeModel.Predict(attackModelInput);
            var predictedAttackValue = byte.Parse(predictedAttackResult.Prediction);

            var attack = attacks.First(attack => attack.AttackValue.Value.Equals(predictedAttackValue));
            return attack;
        }
    }
}
