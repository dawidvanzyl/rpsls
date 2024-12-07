using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using rpsls.Infrastructure.ValueMaps;
using System.Data;

namespace rpsls.Infrastructure.Repositories.Abstracts
{
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

        protected async Task ExecuteAsync(StoredProcedures storedProcedure, object param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = await connection.BeginTransactionAsync())
                {
                    await connection.ExecuteAsync(storedProcedure.Name, param, transaction, commandType: CommandType.StoredProcedure);
                    await transaction.CommitAsync();
                }
            }
        }
    }
}