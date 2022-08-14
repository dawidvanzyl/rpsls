using rpsls.Domain.Values;
using rpsls.Infrastructure.Algorithms.Contexts;
using rpsls.Infrastructure.Repositories;

namespace rpsls.Infrastructure.Factories
{
    public interface IAlgorithmContextFactory
    {
        AlgorithmContext CreateAlgorithmContext(GameValue gameValue);

        MLAlgorithmContext CreateMLAlgorithmContext(GameValue gameValue);
    }

    public class AlgorithmContextFactory : IAlgorithmContextFactory
    {
        private readonly IMatchRepository matchRepository;

        public AlgorithmContextFactory(IMatchRepository matchRepository)
        {
            this.matchRepository = matchRepository;
        }

        public AlgorithmContext CreateAlgorithmContext(GameValue gameValue)
        {
            return CreateAlgorithmContext<AlgorithmContext>(gameValue);
        }

        public MLAlgorithmContext CreateMLAlgorithmContext(GameValue gameValue)
        {
            return CreateAlgorithmContext<MLAlgorithmContext>(gameValue);
        }

        private TAlgorithmContext CreateAlgorithmContext<TAlgorithmContext>(GameValue gameValue)
                    where TAlgorithmContext : AlgorithmContext, new()
        {
            var context = new TAlgorithmContext();

            var previousMatches = matchRepository.GetAllByGameName(gameValue.Name);
            context.LoadPreviousMatches(gameValue, previousMatches);

            return context;
        }
    }
}