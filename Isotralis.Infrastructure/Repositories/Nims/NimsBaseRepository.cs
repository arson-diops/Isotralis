using Dapper;
using Oracle.ManagedDataAccess.Client;
using NLog;

namespace Isotralis.Infrastructure.Repositories.Nims;

public abstract class NimsBaseRepository
{
    private protected readonly string ConnectionString;
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private protected NimsBaseRepository(string? connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        ConnectionString = connectionString;
    }

    private async Task<OracleConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            OracleConnection connection = new(ConnectionString);
            await connection.OpenAsync(cancellationToken);
            return connection;
        }
        catch (OracleException ex)
        {
            Logger.Error(ex, "Database connection error: {Message}", ex.Message);
            throw new RepositoryException("Failed to establish a database connection.", ex);
        }
    }

    internal async Task<IEnumerable<T>> GetQueryResultsAsync<T>(CommandDefinition cmdDefinition)
    {
        try
        {
            await using OracleConnection connection = await GetOpenConnectionAsync(cmdDefinition.CancellationToken);
            return await connection.QueryAsync<T>(cmdDefinition);
        }
        catch (OracleException ex)
        {
            Logger.Error(ex, "Query execution error: {CommandText}", cmdDefinition.CommandText);
            throw new RepositoryException("Failed to execute query.", ex);
        }
    }

    internal async Task<T?> GetSingleQueryResultAsync<T>(CommandDefinition cmdDefinition)
    {
        try
        {
            await using OracleConnection connection = await GetOpenConnectionAsync(cmdDefinition.CancellationToken);
            return await connection.QuerySingleOrDefaultAsync<T>(cmdDefinition);
        }
        catch (OracleException ex)
        {
            Logger.Error(ex, "Single query execution error: {CommandText}", cmdDefinition.CommandText);
            throw new RepositoryException("Failed to retrieve a single query result.", ex);
        }
    }

    internal async Task<int> ExecuteNonQueryAsync(CommandDefinition cmdDefinition)
    {
        try
        {
            await using OracleConnection connection = await GetOpenConnectionAsync(cmdDefinition.CancellationToken);
            return await connection.ExecuteAsync(cmdDefinition);
        }
        catch (OracleException ex)
        {
            Logger.Error(ex, "Non-query execution error: {CommandText}", cmdDefinition.CommandText);
            throw new RepositoryException("Failed to execute a non-query command.", ex);
        }
    }
}
