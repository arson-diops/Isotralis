using Dapper;
using Microsoft.AspNetCore.Http;
using Oracle.ManagedDataAccess.Client;

namespace Isotralis.Infrastructure.Repositories;

public abstract class BaseRepository
{
    private protected readonly string ConnectionString;

    private protected BaseRepository(string? connectionString)
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
        catch (OracleException ex) when (ex.Number == 50000)
        {
            // Log the error and rethrow for handling elsewhere
            Console.WriteLine($"OracleException occurred: {ex.Message}");
            throw new InvalidOperationException("An application-specific error occurred (ORA-50000).", ex);
        }
    }

    internal async Task<IEnumerable<T>> GetQueryResultsAsync<T>(CommandDefinition cmdDefinition)
    {
        await using OracleConnection connection = await GetOpenConnectionAsync(cmdDefinition.CancellationToken);

        return await connection.QueryAsync<T>(cmdDefinition);
    }

    internal async Task<T?> GetSingleQueryResultAsync<T>(CommandDefinition cmdDefinition)
    {
        await using OracleConnection connection = await GetOpenConnectionAsync(cmdDefinition.CancellationToken);

        return await connection.QuerySingleOrDefaultAsync<T>(cmdDefinition);
    }

    internal async Task<T?> GetFirstQueryResultAsync<T>(CommandDefinition cmdDefinition)
    {
        await using OracleConnection connection = await GetOpenConnectionAsync(cmdDefinition.CancellationToken);

        return await connection.QueryFirstOrDefaultAsync<T>(cmdDefinition);
    }

    internal async Task<int> ExecuteNonQueryAsync(CommandDefinition cmdDefinition)
    {
        await using OracleConnection connection = await GetOpenConnectionAsync(cmdDefinition.CancellationToken);

        return await connection.ExecuteAsync(cmdDefinition);
    }

    internal async Task<IEnumerable<TReturn>> GetQueryResultsWithJoinsAsync<TReturn, TSecond>(CommandDefinition cmdDefinition, Func<TReturn, TSecond, TReturn> mapFunction, string splitOn)
    {
        await using OracleConnection connection = await GetOpenConnectionAsync(cmdDefinition.CancellationToken);

        return await connection.QueryAsync(cmdDefinition, mapFunction, splitOn);
    }

    internal async Task<IEnumerable<TReturn>> GetQueryResultsWithJoinsAsync<TReturn, TSecond, TThird>(CommandDefinition cmdDefinition, Func<TReturn, TSecond, TThird, TReturn> mapFunction, IEnumerable<string> splitOns)
    {
        await using OracleConnection connection = await GetOpenConnectionAsync(cmdDefinition.CancellationToken);

        return await connection.QueryAsync(cmdDefinition, mapFunction, string.Join(',', splitOns));
    }
}
