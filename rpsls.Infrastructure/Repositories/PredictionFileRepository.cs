using LiteDB;
using rpsls.Infrastructure.Algorithms.Models;
using rpsls.Infrastructure.Factories;

namespace rpsls.Infrastructure.Repositories
{
    public interface IPredictionRepository
    {
        void AddAIPrediction(MLChallengeModel model);

        void AddPlayerPrediction(MLPlayerAttackModel model);
    }

    public class PredictionFileRepository : IPredictionRepository
    {
        private readonly string connectionString;

        public PredictionFileRepository(IConfigurationFactory configurationFactory)
        {
            var configuration = configurationFactory.GetConfiguration();
            connectionString = configuration.GetSection("ConnectionStrings:MLDb").Value;
        }

        public void AddAIPrediction(MLChallengeModel model)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var collection = db.GetCollection<MLChallengeModel>($"AIPredictions");

                collection.Insert(model);
            }
        }

        public void AddPlayerPrediction(MLPlayerAttackModel model)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var collection = db.GetCollection<MLPlayerAttackModel>($"PlayerPredictions");

                collection.Insert(model);
            }
        }
    }
}