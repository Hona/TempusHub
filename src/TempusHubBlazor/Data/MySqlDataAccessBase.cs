using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.FluentMap;
using MySql.Data.MySqlClient;
using TempusHubBlazor.Logging;

namespace TempusHubBlazor.Data
{
    public class MySqlDataAccessBase : IDisposable
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;

        protected MySqlDataAccessBase(string connectionString)
        {
            _connectionString = connectionString;
            CheckConnectionAsync().GetAwaiter().GetResult();
        }

        private async Task OpenConnectionAsync()
        {
            Logger.LogInfo("Openning MySQL connection");
            if (_connection == null) _connection = new MySqlConnection(_connectionString);
            await _connection.OpenAsync();
        }

        private async Task CheckConnectionAsync()
        {
            if (_connection == null || _connection.State == ConnectionState.Closed ||
                _connection.State == ConnectionState.Broken)
                await OpenConnectionAsync();
        }

        private async Task CloseAsync()
        {
            if (_connection == null) return;
            await _connection.CloseAsync();
            await _connection.ClearAllPoolsAsync();
            _connection.Dispose();
            _connection = null;
        }

        protected async Task<List<T>> QueryAsync<T>(string query)
        {
            try
            {
                await CheckConnectionAsync();
                var result = (await _connection.QueryAsync<T>(query)).ToList();
                return result;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return null;
            }
        }

        protected async Task<List<T>> QueryAsync<T>(string query, object param)
        {
            try
            {
                await CheckConnectionAsync();
                var result = (await _connection.QueryAsync<T>(query, param)).ToList();
                return result;
            }
            catch (Exception e)
            {
                Logger.LogException(e);
                return null;
            }
        }

        protected async Task ExecuteAsync(string query, object param)
        {
            try
            {
                await CheckConnectionAsync();
                await _connection.ExecuteAsync(query, param);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
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