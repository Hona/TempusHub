using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Dapper.FluentMap;
using MySql.Data.MySqlClient;
using TempusHubBlazor.Logging;

namespace TempusHubBlazor.Data
{
    public class MySqlDataAccessBase : IDisposable
    {
        private SemaphoreSlim _queryLock = new SemaphoreSlim(1, 1);
        private readonly string _connectionString;
        private MySqlConnection _connection;

        protected MySqlDataAccessBase(string connectionString)
        {
            _connectionString = connectionString;
            CheckConnectionAsync().GetAwaiter().GetResult();
        }

        private async Task OpenConnectionAsync()
        {
            Logger.LogInfo("Opening MySQL connection");
            _connection ??= new MySqlConnection(_connectionString);
            await _connection.OpenAsync().ConfigureAwait(false);
        }

        private async Task CheckConnectionAsync()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed ||
                _connection.State == ConnectionState.Broken)
                await OpenConnectionAsync().ConfigureAwait(false);
        }

        private async Task CloseAsync()
        {
            if (_connection == null) return;
            await _connection.CloseAsync().ConfigureAwait(false);
            await _connection.ClearAllPoolsAsync().ConfigureAwait(false);
            _connection.Dispose();
            _connection = null;
        }

        protected async Task<List<T>> QueryAsync<T>(string query)
        {
            try
            {
                await _queryLock.WaitAsync().ConfigureAwait(false);
                await CheckConnectionAsync().ConfigureAwait(false);
                var result = (await _connection.QueryAsync<T>(query).ConfigureAwait(false)).ToList();
                return result;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return null;
            }
            finally
            {
                _queryLock.Release();
            }
        }

        protected async Task<List<T>> QueryAsync<T>(string query, object param)
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
                Logger.LogException(e);
                return null;
            }
            finally
            {
                _queryLock.Release();
            }
        }

        protected async Task ExecuteAsync(string query, object param)
        {
            try
            {
                await _queryLock.WaitAsync().ConfigureAwait(false);
                await CheckConnectionAsync().ConfigureAwait(false);
                await _connection.ExecuteAsync(query, param).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
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
}