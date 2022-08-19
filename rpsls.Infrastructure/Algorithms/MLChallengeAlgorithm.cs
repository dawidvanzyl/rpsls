using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;
using rpsls.Infrastructure.Algorithms.Models;
using rpsls.Infrastructure.Repositories;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public class MLChallengeAlgorithm : AbstractChallengeAlgorithm<MLAlgorithmContext>
    {
        private readonly MLContext mlContext;
        private readonly IPredictionRepository predictionRepository;
        private readonly RandomChallengeAlgorithm randomChallengeAlgorithm;
        private MatrixFactorizationTrainer challengeTrainer;
        private MatrixFactorizationTrainer playerTrainer;
        private MatrixFactorizationTrainer.Options playerTrainerOptions;

        public MLChallengeAlgorithm(RandomChallengeAlgorithm randomChallengeAlgorithm, IPredictionRepository predictionRepository)
        {
            mlContext = new MLContext();

            CreateChallangeTrainer();
            CreatePlayerTrainerOptions();

            this.randomChallengeAlgorithm = randomChallengeAlgorithm;
            this.predictionRepository = predictionRepository;
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
            var challengeOptions = new MatrixFactorizationTrainer.Options();
            challengeOptions.MatrixColumnIndexColumnName = nameof(MLModel.Player);
            challengeOptions.MatrixRowIndexColumnName = nameof(MLModel.AI);
            challengeOptions.LabelColumnName = nameof(MLModel.Score);
            challengeOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            challengeOptions.Alpha = 0.01;
            challengeOptions.Lambda = 0.025;
            challengeOptions.Quiet = true;

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

        private void CreatePlayerTrainerOptions()
        {
            playerTrainerOptions = new MatrixFactorizationTrainer.Options();
            playerTrainerOptions.MatrixColumnIndexColumnName = nameof(MLModel.Player);
            playerTrainerOptions.MatrixRowIndexColumnName = nameof(MLModel.AI);
            playerTrainerOptions.LabelColumnName = nameof(MLModel.Score);
            playerTrainerOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            playerTrainerOptions.ApproximationRank = 10;
            playerTrainerOptions.LearningRate = 10;
            playerTrainerOptions.Alpha = 0.01;
            playerTrainerOptions.Lambda = 0.025;
            playerTrainerOptions.Quiet = true;

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

            predictionRepository.AddAIPrediction(aiPrediction);

            var challange = gameValue.Challenges.First(challange => challange.Attack.Value == aiPrediction.AI);
            return challange;
        }

        private ChallengeValue GetPlayerChallenge(PredictionEngine<MLModel, MLPrediction> playerPredictionEngine)
        {
            //var aiChallange = gameValue.Challenges.SingleOrDefault(challange => challange.Attack.Value == ai);

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

            predictionRepository.AddPlayerPrediction(playerPrediction);

            var challange = gameValue.Challenges.First(challange => challange.Attack.Value == playerPrediction.Player);
            return challange;
        }
    }
}