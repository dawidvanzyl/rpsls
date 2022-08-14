using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;
using rpsls.Infrastructure.Algorithms.Models;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public class MLChallengeAlgorithm : AbstractChallengeAlgorithm<MLAlgorithmContext>
    {
        private readonly MLContext mlContext;
        private readonly RandomChallengeAlgorithm randomChallengeAlgorithm;
        private MatrixFactorizationTrainer.Options gameTrainerOptions;
        private IList<(MLModel Model, ChallengeResult ChallengeResult)> trainingModels;
        private MatrixFactorizationTrainer winTrainer;

        public MLChallengeAlgorithm(RandomChallengeAlgorithm randomChallengeAlgorithm)
        {
            mlContext = new MLContext();

            CreateAttackTrainer();
            CreateGameTrainerOptions();

            this.randomChallengeAlgorithm = randomChallengeAlgorithm;
        }

        public override ChallengeValue GetChallenge(MLAlgorithmContext context)
        {
            var predictedPlayerChallenge = GetPredictedPlayerChallenge(context);

            if (predictedPlayerChallenge == null)
            {
                return randomChallengeAlgorithm.GetChallenge(context);
            }

            var aiChallenge = GetAIChallenge(context, predictedPlayerChallenge);

            return aiChallenge == null
                ? randomChallengeAlgorithm.GetChallenge(context)
                : aiChallenge;
        }

        public override void SetupChallenges(ChallengeValue[] challenges)
        {
            base.SetupChallenges(challenges);

            randomChallengeAlgorithm.SetupChallenges(challenges);

            trainingModels = challenges
                .SelectMany(player => challenges
                    .Select(ai =>
                    (
                        new MLModel { Player = player.Attack.Value, AI = ai.Attack.Value },
                        (ChallengeResult)player.CompareTo(ai)
                    )))
                .ToList();
        }

        private void CreateAttackTrainer()
        {
            var winOptions = new MatrixFactorizationTrainer.Options();
            winOptions.MatrixColumnIndexColumnName = nameof(MLModel.Player);
            winOptions.MatrixRowIndexColumnName = nameof(MLModel.AI);
            winOptions.LabelColumnName = nameof(MLModel.Label);
            winOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            winOptions.Alpha = 0.01;
            winOptions.Lambda = 0.025;
            winOptions.Quiet = true;

            winTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(winOptions);
        }

        private void CreateGameTrainerOptions()
        {
            gameTrainerOptions = new MatrixFactorizationTrainer.Options();
            gameTrainerOptions.MatrixColumnIndexColumnName = nameof(MLModel.Player);
            gameTrainerOptions.MatrixRowIndexColumnName = nameof(MLModel.AI);
            gameTrainerOptions.LabelColumnName = nameof(MLModel.Label);
            gameTrainerOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            gameTrainerOptions.Quiet = true;
            //For better results use the following parameters
            //options.ApproximationRank = 100;
            //options.C = 0.00001;
        }

        private PredictionEngine<MLModel, MLPrediction> CreatePredictionEngine(IEnumerable<MLModel> models, MatrixFactorizationTrainer trainer)
        {
            var gameSchema = SchemaDefinition.Create(typeof(MLModel));
            var dataView = mlContext.Data.LoadFromEnumerable(models, gameSchema);
            ITransformer transformer = trainer.Fit(dataView);
            return mlContext.Model.CreatePredictionEngine<MLModel, MLPrediction>(transformer);
        }

        private ChallengeValue GetAIChallenge(MLAlgorithmContext context, ChallengeValue expectedChallenge)
        {
            var winModels = trainingModels
                .Where(trainingModel => trainingModel.ChallengeResult == ChallengeResult.Lost)
                .Select(trainingModel => trainingModel.Model);

            context.AIWins
                .AddRange(winModels);

            var winPredictionEngine = CreatePredictionEngine(context.AIWins, winTrainer);

            var predictions = new List<(float Score, ChallengeValue Challenge)>();
            foreach (var ai in challenges)
            {
                var prediction = winPredictionEngine.Predict(new MLModel
                {
                    Player = expectedChallenge.Attack.Value,
                    AI = ai.Attack.Value
                });

                predictions.Add((prediction.Score, ai));
            }

            var orderedPredictions = predictions.OrderByDescending(item => item.Score);
            return orderedPredictions
                .FirstOrDefault()
                .Challenge;
        }

        private ChallengeValue GetPredictedPlayerChallenge(MLAlgorithmContext context)
        {
            var challengeModels = trainingModels
                .Select(trainingModel => trainingModel.Model);

            context.MLChallengeModels
                .AddRange(challengeModels);

            gameTrainerOptions.Alpha = (double)((context.MLChallengeModels.Count * 2) - context.AIWins.Count) / (double)context.AIWins.Count;
            gameTrainerOptions.Lambda = (double)context.AIWins.Count / 10;

            var gameTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(gameTrainerOptions);

            var playerPredictionEngine = CreatePredictionEngine(context.MLChallengeModels, gameTrainer);

            var predictions = new List<(float Score, ChallengeValue Challenge)>();
            foreach (var player in challenges)
            {
                foreach (var ai in challenges)
                {
                    var challengeEntry = new MLModel
                    {
                        Player = player.Attack.Value,
                        AI = ai.Attack.Value
                    };

                    var prediction = playerPredictionEngine.Predict(challengeEntry);
                    predictions.Add((prediction.Score, player));
                }
            }

            var orderedPredictions = predictions.OrderByDescending(item => item.Score);
            return orderedPredictions
                .FirstOrDefault()
                .Challenge;
        }
    }
}