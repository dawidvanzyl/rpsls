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

        IList<GameRound> GetAllByGameName(string gameName);
    }

    public class MatchFileRepository : IMatchRepository
    {
        private readonly string connectionString;

        public MatchFileRepository(IConfigurationFactory configurationFactory)
        {
            var configuration = configurationFactory.GetConfiguration();
            connectionString = configuration.GetSection("ConnectionStrings:MatchDb").Value;
        }

        public void Add(GameRound matchResult)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var collection = db.GetCollection<GameRound>($"Matches");

                collection.EnsureIndex(result => result.Id, unique: true);

                collection.Insert(matchResult);
            }
        }

        public IList<GameRound> GetAllByGameName(string gameName)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var collection = db.GetCollection<GameRound>($"Matches");

                return collection
                    .FindAll()
                    .Where(gameRound => gameRound.GameName == gameName)
                    .ToList();
            }
        }
    }
}