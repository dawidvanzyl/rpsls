using Dapper;
using Microsoft.Extensions.Configuration;
using rpsls.Entities;
using rpsls.Infrastructure.Dtos;
using rpsls.Infrastructure.Repositories.Abstracts;

namespace rpsls.Infrastructure.Repositories
{
    public interface IGameRepository
    {
        Task CreateMatchResultsAsync(IEnumerable<MatchResult> matchResults);

        Task<IList<MatchResult>> GetMatchResultsAsync();
    }

    public class GameRepository : AbstractRepository, IGameRepository
    {
        public GameRepository(IConfiguration configuration)
            : base(configuration)
        {
        }

        public async Task CreateMatchResultsAsync(IEnumerable<MatchResult> matchResults)
        {
            var tvpMatchResult = DataTableFactory
                .CreateMatchResultsTable(matchResults)
                .AsTableValuedParameter("dbo.tvp_MatchResult");

            var param = new
            {
                MatchResults = tvpMatchResult
            };

            await ExecuteAsync("dbo.CreateMatchResults", param);
        }

        public async Task<IList<MatchResult>> GetMatchResultsAsync()
        {
            var resuls = await QueryAsync<MatchResultDto>("dbo.GetMatchResults");
            return resuls
                .Select(dto => new MatchResult(
                    dto.AttackCount,
                    false,
                    dto.Player1,
                    dto.Player2,
                    dto.Result))
                .ToList();
        }
    }
}