using LiteDB;
using rpsls.Domain;
using rpsls.Infrastructure.Factories;
using System.Collections.Generic;
using System.Linq;

namespace rpsls.Infrastructure.Repositories
{
    public interface IMatchRepository
    {
        void Add(GameRound matchResult);

        IList<GameRound> GetAll();
    }

    public class MatchFileRepository : IMatchRepository
    {
        private readonly string connectionString;

        public MatchFileRepository(IConfigurationFactory configurationFactory)
        {
            var configuration = configurationFactory.GetConfiguration();
            connectionString = configuration.GetSection("ConnectionStrings:FileDb").Value;
        }

        public void Add(GameRound matchResult)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var collection = db.GetCollection<GameRound>("matchResults");

                collection.EnsureIndex(result => result.Id, unique: true);

                collection.Insert(matchResult);
            }
        }

        public IList<GameRound> GetAll()
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var collection = db.GetCollection<GameRound>("matchResults");

                return collection
                    .FindAll()
                    .ToList();
            }
        }
    }
}