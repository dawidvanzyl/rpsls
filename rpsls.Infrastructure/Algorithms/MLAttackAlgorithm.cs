using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using rpsls.Domain;
using rpsls.Domain.Enums;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public class Attack_prediction
    {
        public float Score { get; set; }
    }

    public class AttackEntry
    {
        [KeyType(count: 262111)]
        public uint AIAttack { get; set; }

        public float Label { get; set; }

        [KeyType(count: 262111)]
        public uint PlayerAttack { get; set; }
    }

    public class MLAttackAlgorithm : AbstractAttackAlgorithm
    {
        private readonly Attack[] attacks;
        private readonly IList<AttackEntry> gameData;
        //private readonly MatrixFactorizationTrainer gameTrainer;
        private readonly MLContext mlContext;
        private readonly IList<AttackEntry> winningData;
        private readonly MatrixFactorizationTrainer winTrainer;
        private PredictionEngine<AttackEntry, Attack_prediction> gamePredictionEngine;
        private PredictionEngine<AttackEntry, Attack_prediction> winPredictionEngine;

        public MLAttackAlgorithm(Attack[] attacks) : base(attacks)
        {
            gameData = new List<AttackEntry>();
            winningData = new List<AttackEntry>();
            CreateDefaultSet(attacks);

            this.attacks = attacks;

            mlContext = new MLContext();

            var winOptions = new MatrixFactorizationTrainer.Options();
            winOptions.MatrixColumnIndexColumnName = nameof(AttackEntry.PlayerAttack);
            winOptions.MatrixRowIndexColumnName = nameof(AttackEntry.AIAttack);
            winOptions.LabelColumnName = nameof(AttackEntry.Label);
            winOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            winOptions.Alpha = 0.01;
            winOptions.Lambda = 0.025;
            winOptions.Quiet = true;

            winTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(winOptions);
        }

        public override Attack GetAttack()
        {
            var predictions = new List<(float, Attack)>();
            foreach (var playerAttack in attacks)
            {
                foreach (var aiAttack in attacks)
                {
                    var attackEntry = new AttackEntry
                    {
                        PlayerAttack = playerAttack.AttackValue.Value,
                        AIAttack = aiAttack.AttackValue.Value
                    };
                    var prediction = gamePredictionEngine.Predict(attackEntry);

                    predictions.Add((prediction.Score, playerAttack));
                }
            }

            var orderedPredictions = predictions.OrderByDescending(item => item.Item1);
            var selectedAttack = orderedPredictions
                .First()
                .Item2;

            predictions = new List<(float, Attack)>();
            foreach (var aiAttack in attacks)
            {
                var prediction = winPredictionEngine.Predict(new AttackEntry
                {
                    PlayerAttack = selectedAttack.AttackValue.Value,
                    AIAttack = aiAttack.AttackValue.Value
                });

                predictions.Add((prediction.Score, aiAttack));
            }

            orderedPredictions = predictions.OrderByDescending(item => item.Item1);
            selectedAttack = orderedPredictions
                .First()
                .Item2;

            return selectedAttack;
        }

        public void SavePlayerAttack(Attack playerAttack, Attack aiAttack)
        {
            var attackEntry = new AttackEntry
            {
                PlayerAttack = playerAttack.AttackValue.Value,
                AIAttack = aiAttack.AttackValue.Value
            };

            gameData.Add(attackEntry);

            var attackResult = (AttackResult)playerAttack.CompareTo(aiAttack);
            if (attackResult == AttackResult.Lost)
            {
                winningData.Add(attackEntry);
            }
        }

        public void TrainContext()
        {
            var gameOptions = new MatrixFactorizationTrainer.Options();
            gameOptions.MatrixColumnIndexColumnName = nameof(AttackEntry.PlayerAttack);
            gameOptions.MatrixRowIndexColumnName = nameof(AttackEntry.AIAttack);
            gameOptions.LabelColumnName = nameof(AttackEntry.Label);
            gameOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            gameOptions.Alpha = (double)(gameData.Count * 2 - winningData.Count) / (double)winningData.Count;
            gameOptions.Lambda = (double)winningData.Count / 10;
            gameOptions.Quiet = true;
            //For better results use the following parameters
            //options.ApproximationRank = 100;
            //options.C = 0.00001;

            var gameTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(gameOptions);

            var gameSchema = SchemaDefinition.Create(typeof(AttackEntry));
            var gameTraining = mlContext.Data.LoadFromEnumerable(gameData, gameSchema);
            ITransformer gameModel = gameTrainer.Fit(gameTraining);
            gamePredictionEngine = mlContext.Model.CreatePredictionEngine<AttackEntry, Attack_prediction>(gameModel);

            var winSchema = SchemaDefinition.Create(typeof(AttackEntry));
            var winTraining = mlContext.Data.LoadFromEnumerable(winningData, winSchema);
            ITransformer winModel = winTrainer.Fit(winTraining);
            winPredictionEngine = mlContext.Model.CreatePredictionEngine<AttackEntry, Attack_prediction>(winModel);
        }

        private void CreateDefaultSet(Attack[] attacks)
        {
            foreach (var humanAttack in attacks)
            {
                foreach (var aiAttack in attacks)
                {
                    var attackEntry = new AttackEntry { PlayerAttack = humanAttack.AttackValue.Value, AIAttack = aiAttack.AttackValue.Value };
                    gameData.Add(attackEntry);

                    var attackResult = (AttackResult)humanAttack.CompareTo(aiAttack);
                    if (attackResult == AttackResult.Lost)
                    {
                        winningData.Add(attackEntry);
                    }
                }
            }
        }
    }
}