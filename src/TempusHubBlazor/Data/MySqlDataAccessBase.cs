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
            var connection = await OpenNewConnectionAsync();

            var result = (await connection.QueryAsync<T>(query)).ToList();

            await CloseAsync(connection);
            return result;

        }

        protected async Task<List<T>> QueryAsync<T>(string query, object param)
        {
            var connection = new MySqlConnection(_connectionString);

            List<T> result;
            result = (await connection.QueryAsync<T>(query, param)).ToList();

            await CloseAsync(connection);
            return result;
            
        }

        protected async Task ExecuteAsync(string query, object param)
        {
            var connection = new MySqlConnection(_connectionString);

            await connection.ExecuteAsync(query, param);

            await CloseAsync(connection);
            
        }

        public void Dispose()
        {
            FluentMapper.EntityMaps.Clear();
        }
    }
}