using rpsls.Infrastructure.Algorithms.Contexts;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Infrastructure.Factories
{
    public interface IAlgorithmContextFactory
    {
        AlgorithmContext CreateAlgorithmContext();

        MLAlgorithmContext CreateMLAlgorithmContext();
    }

    public class AlgorithmContextFactory : IAlgorithmContextFactory
    {
        private readonly IMatchRepository matchRepository;

        public AlgorithmContextFactory(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        public AlgorithmContext CreateAlgorithmContext()
        {
            var context = new AlgorithmContext(matchRepository);
            context.LoadMatchResults();

            return context;
        }

        public MLAlgorithmContext CreateMLAlgorithmContext()
        {
            var context = new MLAlgorithmContext(matchRepository);
            context.LoadMatchResults();

            return context;
        }
    }
}