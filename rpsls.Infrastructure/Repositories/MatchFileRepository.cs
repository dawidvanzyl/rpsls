using LiteDB;
using rpsls.Domain;
using rpsls.Infrastructure.Factories;

namespace rpsls.Infrastructure.Repositories
{
    public interface IMatchRepository
    {
        void Add(MatchResult matchResult);
    }

    public class MatchFileRepository : IMatchRepository
    {
        private readonly string connectionString;

        public MatchFileRepository(IConfigurationFactory configurationFactory)
        {
            var configuration = configurationFactory.GetConfiguration();
            connectionString = configuration.GetSection("ConnectionStrings:FileDb").Value;
        }

        public void Add(MatchResult matchResult)
        {
            using (var db = new LiteDatabase(connectionString))
            {
                var collection = db.GetCollection<MatchResult>("matchResults");

                collection.EnsureIndex(result => result.Id, unique: true);

                collection.Insert(matchResult);
            }
        }
    }
}