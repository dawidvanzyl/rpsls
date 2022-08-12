using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public class ChallangeEntry
    {
        [KeyType(count: 262111)]
        public uint AI { get; set; }

        public float Label { get; set; }

        [KeyType(count: 262111)]
        public uint Player { get; set; }
    }

    public class ChallangePrediction
    {
        public float Score { get; set; }
    }

    public class MLChallangeAlgorithm : AbstractChallangeAlgorithm
    {
        private readonly IList<ChallangeEntry> gameChallanges;
        private readonly MLContext mlContext;
        private readonly IList<ChallangeEntry> winningChallanges;
        private readonly MatrixFactorizationTrainer winTrainer;
        private PredictionEngine<ChallangeEntry, ChallangePrediction> gamePredictionEngine;
        private PredictionEngine<ChallangeEntry, ChallangePrediction> winPredictionEngine;

        public MLChallangeAlgorithm(ChallangeValue[] challanges)
            : base(challanges)
        {
            gameChallanges = new List<ChallangeEntry>();
            winningChallanges = new List<ChallangeEntry>();
            CreateDefaultSet(challanges);

            mlContext = new MLContext();

            var winOptions = new MatrixFactorizationTrainer.Options();
            winOptions.MatrixColumnIndexColumnName = nameof(ChallangeEntry.Player);
            winOptions.MatrixRowIndexColumnName = nameof(ChallangeEntry.AI);
            winOptions.LabelColumnName = nameof(ChallangeEntry.Label);
            winOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            winOptions.Alpha = 0.01;
            winOptions.Lambda = 0.025;
            winOptions.Quiet = true;

            winTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(winOptions);
        }

        public override ChallangeValue GetChallange()
        {
            var predictions = new List<(float Score, ChallangeValue Challange)>();
            foreach (var player in challanges)
            {
                foreach (var ai in challanges)
                {
                    var challangeEntry = new ChallangeEntry
                    {
                        Player = player.Attack.Value,
                        AI = ai.Attack.Value
                    };
                    var prediction = gamePredictionEngine.Predict(challangeEntry);

                    predictions.Add((prediction.Score, player));
                }
            }

            var orderedPredictions = predictions.OrderByDescending(item => item.Score);
            var expectedChallange = orderedPredictions
                .First()
                .Challange;

            predictions = new List<(float, ChallangeValue)>();
            foreach (var ai in challanges)
            {
                var prediction = winPredictionEngine.Predict(new ChallangeEntry
                {
                    Player = expectedChallange.Attack.Value,
                    AI = ai.Attack.Value
                });

                predictions.Add((prediction.Score, ai));
            }

            orderedPredictions = predictions.OrderByDescending(item => item.Score);
            var respondingChallange = orderedPredictions
                .First()
                .Challange;

            return respondingChallange;
        }

        public void SaveChallanges(ChallangeValue player, ChallangeValue ai)
        {
            var challangeEntry = new ChallangeEntry
            {
                Player = player.Attack.Value,
                AI = ai.Attack.Value
            };

            gameChallanges.Add(challangeEntry);

            var challangeResult = (ChallangeResult)player.CompareTo(ai);
            if (challangeResult == ChallangeResult.Lost)
            {
                winningChallanges.Add(challangeEntry);
            }
        }

        public void TrainContext()
        {
            var gameOptions = new MatrixFactorizationTrainer.Options();
            gameOptions.MatrixColumnIndexColumnName = nameof(ChallangeEntry.Player);
            gameOptions.MatrixRowIndexColumnName = nameof(ChallangeEntry.AI);
            gameOptions.LabelColumnName = nameof(ChallangeEntry.Label);
            gameOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            gameOptions.Alpha = (double)(gameChallanges.Count * 2 - winningChallanges.Count) / (double)winningChallanges.Count;
            gameOptions.Lambda = (double)winningChallanges.Count / 10;
            gameOptions.Quiet = true;
            //For better results use the following parameters
            //options.ApproximationRank = 100;
            //options.C = 0.00001;

            var gameTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(gameOptions);

            var gameSchema = SchemaDefinition.Create(typeof(ChallangeEntry));
            var gameTraining = mlContext.Data.LoadFromEnumerable(gameChallanges, gameSchema);
            ITransformer gameModel = gameTrainer.Fit(gameTraining);
            gamePredictionEngine = mlContext.Model.CreatePredictionEngine<ChallangeEntry, ChallangePrediction>(gameModel);

            var winSchema = SchemaDefinition.Create(typeof(ChallangeEntry));
            var winTraining = mlContext.Data.LoadFromEnumerable(winningChallanges, winSchema);
            ITransformer winModel = winTrainer.Fit(winTraining);
            winPredictionEngine = mlContext.Model.CreatePredictionEngine<ChallangeEntry, ChallangePrediction>(winModel);
        }

        private void CreateDefaultSet(ChallangeValue[] challanges)
        {
            foreach (var player in challanges)
            {
                foreach (var ai in challanges)
                {
                    var challangeEntry = new ChallangeEntry { Player = player.Attack.Value, AI = ai.Attack.Value };
                    gameChallanges.Add(challangeEntry);

                    var challangeResult = (ChallangeResult)player.CompareTo(ai);
                    if (challangeResult == ChallangeResult.Lost)
                    {
                        winningChallanges.Add(challangeEntry);
                    }
                }
            }
        }
    }
}