using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace DatabaseScriptExecutor
{
    public class FormRepository : IFormRepository
    {
        public async Task<IEnumerable<string>> GetDbNamesAsync(string connectionString)
        {
            const string query = "SELECT name FROM sys.databases where name NOT IN('master', 'model', 'msdb', 'tempdb') AND sys.databases.state = 0";
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var dbs = await conn.QueryAsync<string>(query);
                return dbs;
            }

        }

        public async Task ExecuteSqlAsync(string connectionString, string dbName, string sql)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                var query = $"use [{dbName}]; \r\n {sql}";
                await conn.ExecuteAsync(query);
            }
        }
    }
}
