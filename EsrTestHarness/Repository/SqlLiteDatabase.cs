using Dapper;
using EsrTestHarness.Model;
using System;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace EsrTestHarness.Repository
{
    public class SqlLiteDatabase : ITestDatabase
    {
        public SqlLiteDatabase()
        {
            if (!File.Exists(DbFile))
            {
                CreateDatabaseAsync().Wait();
            }
        }

        public static string DbFile => $"{Environment.CurrentDirectory}\\EsrTestHarness.sqlite";

        public static SQLiteConnection DbConnection() => new SQLiteConnection("Data Source=" + DbFile);

        private static async Task CreateDatabaseAsync()
        {
            using var connection = DbConnection();

            await connection.ExecuteAsync(@"create table User
                (
                    Id integer primary key AUTOINCREMENT,
                    Email varchar(100) not null,
                    SignalRHubConnectionId string null
                )");
        }

        public async Task<User> GetUserAsync(string email)
        {
            using var connection = DbConnection();

            return await connection.QueryFirstOrDefaultAsync<User>($"SELECT * FROM User WHERE Email = @email LIMIT 1", new { email });
        }

        public async Task InsertUserAsync(User user)
        {
            using var connection = DbConnection();

            await connection.ExecuteAsync(@"INSERT INTO User (Email, SignalRHubConnectionId) VALUES (@Email, @SignalRHubConnectionId);", user);
        }

        public async Task UpdateUserAsync(string email, string signalRHubConnectionId)
        {
            using var connection = DbConnection();

            await connection.ExecuteAsync($"UPDATE User Set SignalRHubConnectionId = @signalRHubConnectionId WHERE Email = @email", new { email, signalRHubConnectionId });
        }
    }
}