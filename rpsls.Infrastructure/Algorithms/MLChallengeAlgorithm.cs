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
        private GameValue gameValue;
        private MatrixFactorizationTrainer.Options playerAttackTrainerOptions;

        public MLChallengeAlgorithm(RandomChallengeAlgorithm randomChallengeAlgorithm, IPredictionRepository predictionRepository)
        {
            mlContext = new MLContext();

            CreateWinTrainer();
            CreatePlayerAttackTrainerOptions();

            this.randomChallengeAlgorithm = randomChallengeAlgorithm;
            this.predictionRepository = predictionRepository;
        }

        public override ChallengeValue GetChallenge(MLAlgorithmContext context)
        {
            var winPredictionEngine = CreateChallengePredictionEngine(context);
            var playerPredictionEngine = CreatePlayerAttackPredictionEngine(context);

            var predictedPlayerChallenge = GetPlayerChallenge(playerPredictionEngine);

            if (predictedPlayerChallenge == null)
            {
                return randomChallengeAlgorithm.GetChallenge(context);
            }

            var aiChallenge = GetAIChallenge(winPredictionEngine, predictedPlayerChallenge);

            return aiChallenge == null
                ? randomChallengeAlgorithm.GetChallenge(context)
                : aiChallenge;
        }

        public override void SetGameValue(GameValue gameValue)
        {
            base.SetGameValue(gameValue);

            this.gameValue = gameValue;

            randomChallengeAlgorithm.SetGameValue(gameValue);
        }

        private PredictionEngine<MLChallengeModel, MLPrediction> CreateChallengePredictionEngine(MLAlgorithmContext context)
        {
            var dataView = mlContext.Data.LoadFromEnumerable(context.MLChallengeModels, SchemaDefinition.Create(typeof(MLChallengeModel)));
            ITransformer transformer = challengeTrainer.Fit(dataView);
            return mlContext.Model.CreatePredictionEngine<MLChallengeModel, MLPrediction>(transformer);
        }

        private PredictionEngine<MLPlayerAttackModel, MLPrediction> CreatePlayerAttackPredictionEngine(MLAlgorithmContext context)
        {
            playerAttackTrainerOptions.Alpha = ((double)context.MLPlayerAttackModels.Count * 2) / (double)context.MLChallengeModels.Count;
            playerAttackTrainerOptions.Lambda = (double)context.MLChallengeModels.Count / 10;

            var trainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(playerAttackTrainerOptions);

            var dataView = mlContext.Data.LoadFromEnumerable(context.MLPlayerAttackModels, SchemaDefinition.Create(typeof(MLPlayerAttackModel)));
            ITransformer transformer = trainer.Fit(dataView);
            return mlContext.Model.CreatePredictionEngine<MLPlayerAttackModel, MLPrediction>(transformer);
        }

        private void CreatePlayerAttackTrainerOptions()
        {
            playerAttackTrainerOptions = new MatrixFactorizationTrainer.Options();
            playerAttackTrainerOptions.MatrixColumnIndexColumnName = nameof(MLPlayerAttackModel.Attack);
            playerAttackTrainerOptions.MatrixRowIndexColumnName = nameof(MLPlayerAttackModel.ChallengeResult);
            playerAttackTrainerOptions.LabelColumnName = nameof(MLChallengeModel.Score);
            playerAttackTrainerOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            playerAttackTrainerOptions.Quiet = true;
            //For better results use the following parameters
            //options.ApproximationRank = 100;
            //options.C = 0.00001;
        }

        private void CreateWinTrainer()
        {
            var challengeOptions = new MatrixFactorizationTrainer.Options();
            challengeOptions.MatrixColumnIndexColumnName = nameof(MLChallengeModel.Player);
            challengeOptions.MatrixRowIndexColumnName = nameof(MLChallengeModel.AI);
            challengeOptions.LabelColumnName = nameof(MLChallengeModel.Score);
            challengeOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            challengeOptions.Alpha = 0.01;
            challengeOptions.Lambda = 0.025;
            challengeOptions.Quiet = true;

            challengeTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(challengeOptions);
        }

        private ChallengeValue GetAIChallenge(PredictionEngine<MLChallengeModel, MLPrediction> winPredictionEngine, ChallengeValue player)
        {
            var predictions = gameValue.Challenges
                .Select(ai =>
                {
                    var model = MLChallengeModel.From(gameValue.Name, player, ai);
                    var prediction = winPredictionEngine.Predict(model);
                    model.Score = prediction.Score;

                    return model;
                });

            var orderedPredictions = predictions.OrderByDescending(item => item.Score);
            var aiPrediction = orderedPredictions.FirstOrDefault();

            predictionRepository.AddAIPrediction(aiPrediction);

            var challange = gameValue.Challenges.First(challange => challange.Attack.Value == aiPrediction.AI);
            return challange;
        }

        private ChallengeValue GetPlayerChallenge(PredictionEngine<MLPlayerAttackModel, MLPrediction> playerPredictionEngine)
        {
            var predictions = gameValue.Challenges
                .SelectMany(player => gameValue.Challenges
                    .Where(ai => (ChallengeResult)player.CompareTo(ai) is ChallengeResult.Won or ChallengeResult.Tied)
                    .Select(ai =>
                    {
                        var model = MLPlayerAttackModel.From(gameValue.Name, player, ai);
                        var prediction = playerPredictionEngine.Predict(model);
                        model.Score = prediction.Score;

                        return model;
                    }));

            var orderedPredictions = predictions.OrderByDescending(item => item.Score);
            var playerPrediction = orderedPredictions.FirstOrDefault();

            predictionRepository.AddPlayerPrediction(playerPrediction);

            var challange = gameValue.Challenges.First(challange => challange.Attack.Value == playerPrediction.Attack);
            return challange;
        }
    }
}