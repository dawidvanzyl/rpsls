using Microsoft.Extensions.Configuration;
using rpsls.Entities;
using rpsls.Infrastructure.Repositories.Abstracts;

namespace rpsls.Infrastructure.Repositories;

public interface IRuleRepository
{
    Task<IList<Rule>> GetAllAsync();
}

public class RuleRepository(IConfiguration configuration)
        : AbstractRepository(configuration), IRuleRepository
{
    public async Task<IList<Rule>> GetAllAsync()
    {
        var result = await QueryAsync<Rule>("dbo.GetRules");
        return result.ToList();
    }
}