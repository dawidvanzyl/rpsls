using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;
using rpsls.Infrastructure.Algorithms.Models;
using System;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public class MLChallengeAlgorithm : AbstractChallengeAlgorithm<MLAlgorithmContext>
    {
        private readonly MLContext mlContext;
        private readonly RandomChallengeAlgorithm randomChallengeAlgorithm;
        private MatrixFactorizationTrainer challengeTrainer;
        private MatrixFactorizationTrainer playerTrainer;

        public MLChallengeAlgorithm(RandomChallengeAlgorithm randomChallengeAlgorithm)
        {
            mlContext = new MLContext();

            this.randomChallengeAlgorithm = randomChallengeAlgorithm;
        }

        public override ChallengeValue GetChallenge(MLAlgorithmContext context)
        {
            var challengePredictionEngine = CreateChallengePredictionEngine(context);
            var playerPredictionEngine = CreatePlayerPredictionEngine(context);

            var predictedPlayerChallenge = GetPlayerChallenge(playerPredictionEngine);

            var challenge = GetChallenge(challengePredictionEngine, predictedPlayerChallenge);

            return challenge;
        }

        public override void SetGameValue(GameValue gameValue)
        {
            base.SetGameValue(gameValue);

            CreateChallangeTrainer();
            CreatePlayerTrainer();

            randomChallengeAlgorithm.SetGameValue(gameValue);
        }

        private void CreateChallangeTrainer()
        {
            var challengeOptions = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(MLModel.Player),
                MatrixRowIndexColumnName = nameof(MLModel.ChallangeResult),
                LabelColumnName = nameof(MLModel.Score),
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                Alpha = 0.01,
                Lambda = 0.025,
                Quiet = true
            };

            challengeTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(challengeOptions);
        }

        private PredictionEngine<MLModel, MLPrediction> CreateChallengePredictionEngine(MLAlgorithmContext context)
        {
            var playerLoses = context.MLModels.Where(model => model.ChallangeResult == 0);
            var data = playerLoses
                .Union(context.MLModels
                    .Where(model => model.ChallangeResult == 2)
                    .Select(model => model.ReverseChallanges(-2)));

            var dataView = mlContext.Data.LoadFromEnumerable(data, SchemaDefinition.Create(typeof(MLModel)));
            ITransformer transformer = challengeTrainer.Fit(dataView);
            return mlContext.Model.CreatePredictionEngine<MLModel, MLPrediction>(transformer);
        }

        private PredictionEngine<MLModel, MLPrediction> CreatePlayerPredictionEngine(MLAlgorithmContext context)
        {
            var data = context.MLModels.TakeLast((int)Math.Pow(gameValue.Challenges.Count, 2));

            var dataView = mlContext.Data.LoadFromEnumerable(data, SchemaDefinition.Create(typeof(MLModel)));
            ITransformer transformer = playerTrainer.Fit(dataView);
            return mlContext.Model.CreatePredictionEngine<MLModel, MLPrediction>(transformer);
        }

        private void CreatePlayerTrainer()
        {
            var playerTrainerOptions = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(MLModel.Player),
                MatrixRowIndexColumnName = nameof(MLModel.Machine),
                LabelColumnName = nameof(MLModel.Score),
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                ApproximationRank = gameValue.Challenges.Count,
                LearningRate = 1,
                Alpha = 0.01,
                Lambda = 0.025,
                Quiet = true
            };

            playerTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(playerTrainerOptions);
        }

        private ChallengeValue GetChallenge(PredictionEngine<MLModel, MLPrediction> challangePredictionEngine, ChallengeValue player)
        {
            var orderedPredictions = gameValue.Challenges
                .Select(machine =>
                {
                    var model = MLModel.From(gameValue.Name, player, machine);
                    var prediction = challangePredictionEngine.Predict(model);
                    model.Score = prediction.Score;

                    return model;
                })
                .OrderByDescending(prediction => prediction.Score);

            var machinePrediction = orderedPredictions.First();
            var challange = gameValue.Challenges.First(challange => challange.Attack.Value == machinePrediction.Machine);
            return challange;
        }

        private ChallengeValue GetPlayerChallenge(PredictionEngine<MLModel, MLPrediction> playerPredictionEngine)
        {
            var orderedPredictions = gameValue.Challenges
                .SelectMany(player => gameValue.Challenges
                    .Select(machine =>
                    {
                        var model = MLModel.From(gameValue.Name, player, machine);
                        var prediction = playerPredictionEngine.Predict(model);
                        model.Score = prediction.Score;

                        return model;
                    }))
                .OrderByDescending(prediction => prediction.Score);

            var playerPrediction = orderedPredictions.First();
            var challange = gameValue.Challenges.First(challange => challange.Attack.Value == playerPrediction.Player);
            return challange;
        }
    }
}