using System.Data;
using Dapper;
using Dapper.FluentMap;
using MySql.Data.MySqlClient;
using Serilog;

namespace TempusHub.Infrastructure;

public class MySqlDataAccessBase : IDisposable
{
    private readonly SemaphoreSlim _queryLock = new(1, 1);
    private readonly string _connectionString;
    private MySqlConnection _connection;

    internal MySqlDataAccessBase(string connectionString)
    {
        _connectionString = connectionString;
        CheckConnectionAsync().GetAwaiter().GetResult();
    }

    private async Task OpenConnectionAsync()
    {
        Log.Information("Opening MySQL connection");
        _connection ??= new MySqlConnection(_connectionString);
        await _connection.OpenAsync().ConfigureAwait(false);
    }

    private async Task CheckConnectionAsync()
    {
        if (_connection == null || _connection.State == ConnectionState.Closed ||
            _connection.State == ConnectionState.Broken)
            await OpenConnectionAsync().ConfigureAwait(false);
    }

    public async Task<List<T>> QueryAsync<T>(string query, object param)
    {
        try
        {
            await _queryLock.WaitAsync().ConfigureAwait(false);
            await CheckConnectionAsync().ConfigureAwait(false);
            var result = (await _connection.QueryAsync<T>(query, param).ConfigureAwait(false)).ToList();
            return result;
        }
        catch (Exception e)
        {
            Log.Error(e, "Unhandled exception while running a MySQL query");
            return null;
        }
        finally
        {
            _queryLock.Release();
        }
    }

    public async Task ExecuteAsync(string query, object param)
    {
        try
        {
            await _queryLock.WaitAsync().ConfigureAwait(false);
            await CheckConnectionAsync().ConfigureAwait(false);
            await _connection.ExecuteAsync(query, param).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            Log.Error(e, "Unhandled exception while running a MySQL command");
        }
        finally
        {
            _queryLock.Release();
        }
    }

    public void Dispose()
    {
        if (_connection != null && _connection.State != ConnectionState.Closed)
        {
            _connection.Dispose();
        }
        FluentMapper.EntityMaps.Clear();
    }
}