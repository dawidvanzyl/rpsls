using Dapper;
using Microsoft.Extensions.Configuration;
using rpsls.Entities;
using rpsls.Infrastructure.Dtos;
using rpsls.Infrastructure.Repositories.Abstracts;

namespace rpsls.Infrastructure.Repositories;

public interface IMatchRepository
{
    Task CreateAsync(IEnumerable<Match> matchResults);

    Task<IList<Match>> GetAllAsync();
}

public class MatchRepository(IConfiguration configuration)
    : AbstractRepository(configuration), IMatchRepository
{
    public async Task CreateAsync(IEnumerable<Match> matchResults)
    {
        var tvpMatchResult = DataTableFactory
            .CreateMatchResultsTable(matchResults)
            .AsTableValuedParameter("dbo.tvp_Match");

        var param = new
        {
            Matches = tvpMatchResult
        };

        await ExecuteAsync("dbo.SaveMatches", param);
    }

    public async Task<IList<Match>> GetAllAsync()
    {
        var resuls = await QueryAsync<MatchDto>("dbo.GetMatches");
        return resuls
            .Select(dto => new Match(
                dto.AttackCount,
                false,
                dto.Player1,
                dto.Player2,
                dto.Result))
            .ToList();
    }
}