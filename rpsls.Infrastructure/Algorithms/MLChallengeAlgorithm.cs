using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;
using rpsls.Infrastructure.Algorithms.Models;
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

            CreateChallangeTrainer();
            CreatePlayerTrainer();

            this.randomChallengeAlgorithm = randomChallengeAlgorithm;
        }

        public override ChallengeValue GetChallenge(MLAlgorithmContext context)
        {
            //if (!context.LastAIChallange.HasValue)
            //{
            //    return randomChallengeAlgorithm.GetChallenge(context);
            //}

            var challangePredictionEngine = CreateChallengePredictionEngine(context);
            var playerPredictionEngine = CreatePlayerPredictionEngine(context);

            var predictedPlayerChallenge = GetPlayerChallenge(playerPredictionEngine);

            var aiChallenge = GetAIChallenge(challangePredictionEngine, predictedPlayerChallenge);

            return aiChallenge;
        }

        public override void SetGameValue(GameValue gameValue)
        {
            base.SetGameValue(gameValue);

            randomChallengeAlgorithm.SetGameValue(gameValue);
        }

        private void CreateChallangeTrainer()
        {
            var challengeOptions = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(MLModel.Player),
                MatrixRowIndexColumnName = nameof(MLModel.AI),
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
            var playerLoses = context.MLModels.Where(model => model.ChallangeResult is (int)ChallengeResult.Lost);

            var dataView = mlContext.Data.LoadFromEnumerable(playerLoses, SchemaDefinition.Create(typeof(MLModel)));
            ITransformer transformer = challengeTrainer.Fit(dataView);
            return mlContext.Model.CreatePredictionEngine<MLModel, MLPrediction>(transformer);
        }

        private PredictionEngine<MLModel, MLPrediction> CreatePlayerPredictionEngine(MLAlgorithmContext context)
        {
            var dataView = mlContext.Data.LoadFromEnumerable(context.MLModels.TakeLast(10), SchemaDefinition.Create(typeof(MLModel)));
            ITransformer transformer = playerTrainer.Fit(dataView);
            return mlContext.Model.CreatePredictionEngine<MLModel, MLPrediction>(transformer);
        }

        private void CreatePlayerTrainer()
        {
            var playerTrainerOptions = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = nameof(MLModel.Player),
                MatrixRowIndexColumnName = nameof(MLModel.AI),
                LabelColumnName = nameof(MLModel.Score),
                LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass,
                ApproximationRank = 10,
                LearningRate = 10,
                Alpha = 0.01,
                Lambda = 0.025,
                Quiet = true
            };

            playerTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(playerTrainerOptions);
        }

        private ChallengeValue GetAIChallenge(PredictionEngine<MLModel, MLPrediction> challangePredictionEngine, ChallengeValue player)
        {
            var orderedPredictions = gameValue.Challenges
                .Select(ai =>
                {
                    var model = MLModel.From(gameValue.Name, player, ai);
                    var prediction = challangePredictionEngine.Predict(model);
                    model.Score = prediction.Score;

                    return model;
                })
                .OrderByDescending(prediction => prediction.Score);

            var aiPrediction = orderedPredictions.FirstOrDefault(prediction => (ChallengeResult)prediction.ChallangeResult == ChallengeResult.Lost);

            var challange = gameValue.Challenges.First(challange => challange.Attack.Value == aiPrediction.AI);
            return challange;
        }

        private ChallengeValue GetPlayerChallenge(PredictionEngine<MLModel, MLPrediction> playerPredictionEngine)
        {
            var orderedPredictions = gameValue.Challenges
                .SelectMany(player => gameValue.Challenges
                    .Select(ai =>
                    {
                        var model = MLModel.From(gameValue.Name, player, ai);
                        var prediction = playerPredictionEngine.Predict(model);
                        model.Score = prediction.Score;

                        return model;
                    }))
                .OrderByDescending(prediction => prediction.Score);

            var playerPrediction = orderedPredictions.FirstOrDefault(prediction => (ChallengeResult)prediction.ChallangeResult == ChallengeResult.Won);

            var challange = gameValue.Challenges.First(challange => challange.Attack.Value == playerPrediction.Player);
            return challange;
        }
    }
}