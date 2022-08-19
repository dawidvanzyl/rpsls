using LiteDB;
using rpsls.Infrastructure.Algorithms.Models;
using rpsls.Infrastructure.Factories;

namespace rpsls.Infrastructure.Repositories
{
    public interface IPredictionRepository
    {
        void AddAIPrediction(MLModel model);

        void AddPlayerPrediction(MLModel model);
    }

    public class PredictionFileRepository : IPredictionRepository
    {
        private readonly string connectionString;

        public PredictionFileRepository(IConfigurationFactory configurationFactory)
        {
            var configuration = configurationFactory.GetConfiguration();
            connectionString = configuration.GetSection("ConnectionStrings:MLDb").Value;
        }

        public void AddAIPrediction(MLModel model)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var collection = db.GetCollection<MLModel>($"AIPredictions");

                collection.Insert(model);
            }
        }

        public void AddPlayerPrediction(MLModel model)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var collection = db.GetCollection<MLModel>($"PlayerPredictions");

                collection.Insert(model);
            }
        }
    }
}