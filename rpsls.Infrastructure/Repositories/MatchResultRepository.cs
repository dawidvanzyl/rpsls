using Dapper;
using Microsoft.Extensions.Configuration;
using rpsls.Entities;
using rpsls.Infrastructure.Dtos;
using rpsls.Infrastructure.Repositories.Abstracts;

namespace rpsls.Infrastructure.Repositories;

public interface IMatchResultRepository
{
    Task CreateAsync(IEnumerable<MatchResult> matchResults);

    Task<IList<MatchResult>> GetAllAsync();
}

public class MatchResultRepository(IConfiguration configuration)
    : AbstractRepository(configuration), IMatchResultRepository
{
    public async Task CreateAsync(IEnumerable<MatchResult> matchResults)
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

    public async Task<IList<MatchResult>> GetAllAsync()
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