using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using static Dapper.SqlMapper;

namespace rpsls.Infrastructure.Repositories.Abstracts;

public abstract class AbstractRepository
{
    private readonly string _connectionString;

    protected AbstractRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DbConnection");
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("Connection not configured");
        }
    }

    protected async Task ExecuteAsync(string storedProcedure, object param = null)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var transaction = await connection.BeginTransactionAsync())
            {
                await connection.ExecuteAsync(storedProcedure, param, transaction, commandType: CommandType.StoredProcedure);
                await transaction.CommitAsync();
            }
        }
    }

    protected async Task<T> ExecuteAsync<T>(string storedProcedure, object param = null)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var transaction = await connection.BeginTransactionAsync())
            {
                var result = await connection.ExecuteScalarAsync<T>(storedProcedure, param, transaction, commandType: CommandType.StoredProcedure);
                await transaction.CommitAsync();

                return result;
            }
        }
    }

    protected async Task<IEnumerable<TResult>> QueryAsync<TResult>(string storedProcedure, object param = null)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            return await connection.QueryAsync<TResult>(storedProcedure, param, commandType: CommandType.StoredProcedure);
        }
    }

    protected async Task<IEnumerable<TResult>> QueryMultipleAsync<TResult>(string storedProcedure, Func<GridReader, IEnumerable<TResult>> gridReaderMap, object param = null)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            var result = await connection.QueryMultipleAsync(storedProcedure, param, commandType: CommandType.StoredProcedure);

            return gridReaderMap(result);
        }
    }
}