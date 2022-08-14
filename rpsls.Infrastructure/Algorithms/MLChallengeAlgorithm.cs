using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using rpsls.Domain.Enums;
using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;
using rpsls.Infrastructure.Algorithms.Models;
using rpsls.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Algorithms
{
    public class MLChallengeAlgorithm : AbstractChallengeAlgorithm<MLAlgorithmContext>
    {
        private readonly MLContext mlContext;
        private readonly IPredictionRepository predictionRepository;
        private readonly RandomChallengeAlgorithm randomChallengeAlgorithm;
        private bool challangeTrainModelsAdded;
        private MatrixFactorizationTrainer.Options gameTrainerOptions;
        private GameValue gameValue;
        private IList<(MLModel Model, ChallengeResult ChallengeResult)> trainingModels;
        private MatrixFactorizationTrainer winTrainer;
        private bool winTrainModelsAdded;

        public MLChallengeAlgorithm(RandomChallengeAlgorithm randomChallengeAlgorithm, IPredictionRepository predictionRepository)
        {
            mlContext = new MLContext();

            CreateWinTrainer();
            CreateGameTrainerOptions();

            this.randomChallengeAlgorithm = randomChallengeAlgorithm;
            this.predictionRepository = predictionRepository;
        }

        public override ChallengeValue GetChallenge(MLAlgorithmContext context)
        {
            var winPredictionEngine = CreateWinPredictionEngine(context);
            var playerPredictionEngine = CreatePlayerPredictionEngine(context);

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

        public override void SetupChallenges(GameValue gameValue)
        {
            base.SetupChallenges(gameValue);

            this.gameValue = gameValue;

            randomChallengeAlgorithm.SetupChallenges(gameValue);

            trainingModels = challenges
                .SelectMany(player => challenges
                    .Select(ai =>
                    (
                        new MLModel { Player = player.Attack.Value, AI = ai.Attack.Value },
                        (ChallengeResult)player.CompareTo(ai)
                    )))
                .ToList();
        }

        private void CreateGameTrainerOptions()
        {
            gameTrainerOptions = new MatrixFactorizationTrainer.Options();
            gameTrainerOptions.MatrixColumnIndexColumnName = nameof(MLModel.Player);
            gameTrainerOptions.MatrixRowIndexColumnName = nameof(MLModel.AI);
            gameTrainerOptions.LabelColumnName = nameof(MLModel.Score);
            gameTrainerOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            gameTrainerOptions.Quiet = true;
            //For better results use the following parameters
            //options.ApproximationRank = 100;
            //options.C = 0.00001;
        }

        private PredictionEngine<MLModel, MLPrediction> CreatePlayerPredictionEngine(MLAlgorithmContext context)
        {
            if (!challangeTrainModelsAdded)
            {
                var challengeModels = trainingModels
                    .Select(trainingModel => trainingModel.Model);

                context.MLChallengeModels
                    .AddRange(challengeModels);

                challangeTrainModelsAdded = true;
            }

            gameTrainerOptions.Alpha = (double)context.MLChallengeModels.Count / (double)context.AIWins.Count;
            gameTrainerOptions.Lambda = (double)context.AIWins.Count / 10;

            var gameTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(gameTrainerOptions);

            var playerPredictionEngine = CreatePredictionEngine(context.MLChallengeModels, gameTrainer);
            return playerPredictionEngine;
        }

        private PredictionEngine<MLModel, MLPrediction> CreatePredictionEngine(IEnumerable<MLModel> models, MatrixFactorizationTrainer trainer)
        {
            var gameSchema = SchemaDefinition.Create(typeof(MLModel));
            var dataView = mlContext.Data.LoadFromEnumerable(models, gameSchema);
            ITransformer transformer = trainer.Fit(dataView);
            return mlContext.Model.CreatePredictionEngine<MLModel, MLPrediction>(transformer);
        }

        private PredictionEngine<MLModel, MLPrediction> CreateWinPredictionEngine(MLAlgorithmContext context)
        {
            if (!winTrainModelsAdded)
            {
                var winModels = trainingModels
                .Where(trainingModel => trainingModel.ChallengeResult == ChallengeResult.Lost)
                .Select(trainingModel => trainingModel.Model);

                context.AIWins
                    .AddRange(winModels);

                winTrainModelsAdded = true;
            }

            var winPredictionEngine = CreatePredictionEngine(context.AIWins, winTrainer);
            return winPredictionEngine;
        }

        private void CreateWinTrainer()
        {
            var winOptions = new MatrixFactorizationTrainer.Options();
            winOptions.MatrixColumnIndexColumnName = nameof(MLModel.Player);
            winOptions.MatrixRowIndexColumnName = nameof(MLModel.AI);
            winOptions.LabelColumnName = nameof(MLModel.Score);
            winOptions.LossFunction = MatrixFactorizationTrainer.LossFunctionType.SquareLossOneClass;
            winOptions.Alpha = 0.01;
            winOptions.Lambda = 0.025;
            winOptions.Quiet = true;

            winTrainer = mlContext
                .Recommendation()
                .Trainers
                .MatrixFactorization(winOptions);
        }

        private ChallengeValue GetAIChallenge(PredictionEngine<MLModel, MLPrediction> winPredictionEngine, ChallengeValue expectedChallenge)
        {
            var predictions = new List<(float Score, ChallengeValue Challenge, MLModel Model)>();
            foreach (var ai in challenges)
            {
                var model = new MLModel
                {
                    GameName = gameValue.Name,
                    Player = expectedChallenge.Attack.Value,
                    AI = ai.Attack.Value
                };

                var prediction = winPredictionEngine.Predict(model);
                model.Score = prediction.Score;
                predictions.Add((prediction.Score, ai, model));
            }

            var orderedPredictions = predictions.OrderByDescending(item => item.Score);
            var aiPrediction = orderedPredictions.FirstOrDefault();

            predictionRepository.AddAIPrediction(aiPrediction.Model);
            return aiPrediction.Challenge;
        }

        private ChallengeValue GetPlayerChallenge(PredictionEngine<MLModel, MLPrediction> playerPredictionEngine)
        {
            var predictions = new List<(float Score, ChallengeValue Challenge, MLModel Model)>();
            foreach (var player in challenges)
            {
                foreach (var ai in challenges)
                {
                    var model = new MLModel
                    {
                        GameName = gameValue.Name,
                        Player = player.Attack.Value,
                        AI = ai.Attack.Value
                    };

                    var prediction = playerPredictionEngine.Predict(model);
                    model.Score = prediction.Score;

                    predictions.Add((prediction.Score, player, model));
                }
            }

            var orderedPredictions = predictions.OrderByDescending(item => item.Score);
            var playerPrediction = orderedPredictions.FirstOrDefault();

            predictionRepository.AddPlayerPrediction(playerPrediction.Model);
            return playerPrediction.Challenge;
        }
    }
}