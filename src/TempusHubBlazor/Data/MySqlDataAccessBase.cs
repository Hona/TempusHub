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
    public class MySqlDataAccessBase
    {
        private readonly string _connectionString;

        protected MySqlDataAccessBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task<MySqlConnection> OpenNewConnectionAsync()
        {
            Logger.LogInfo("Openning MySQL connection");
            var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            return connection;
        }

        private async Task CloseAsync(MySqlConnection connection)
        {
            if (connection == null) return;
            await connection.CloseAsync();
            await connection.ClearAllPoolsAsync();
            connection.Dispose();
        }

        protected async Task<List<T>> QueryAsync<T>(string query)
        {
            try
            {
                var connection = await OpenNewConnectionAsync();
                var result = (await connection.QueryAsync<T>(query)).ToList();
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
                var connection = new MySqlConnection(_connectionString);
                var result = (await connection.QueryAsync<T>(query, param)).ToList();
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
                var connection = new MySqlConnection(_connectionString);
                await connection.ExecuteAsync(query, param);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        public void Dispose()
        {
            FluentMapper.EntityMaps.Clear();
        }
    }
}