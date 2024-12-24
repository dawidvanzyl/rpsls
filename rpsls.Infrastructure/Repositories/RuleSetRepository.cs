using Microsoft.Extensions.Configuration;
using rpsls.Entities;
using rpsls.Infrastructure.Repositories.Abstracts;

namespace rpsls.Infrastructure.Repositories;

public interface IRuleSetRepository
{
    Task<IList<RuleSet>> GetAsync();
}

public class RuleSetRepository(IConfiguration configuration)
        : AbstractRepository(configuration), IRuleSetRepository
{
    public async Task<IList<RuleSet>> GetAsync()
    {
        var result = await QueryAsync<RuleSet>("dbo.GetRuleSet");
        return result.ToList();
    }
}