using Azure;
using Azure.Data.Tables;
using EsrTestHarness.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EsrTestHarness.Repository
{
    public class AzureTableDatabase : ITestDatabase
    {
        private readonly TableClient _tableClient;
        private const string PartitionKey = "EsrTestHarness";

        public AzureTableDatabase(IOptions<AppSettings> settings)
        {
            _tableClient = new TableClient(settings.Value.StorageConnectionString, "EsrTestHarnessUsers");
            _tableClient.CreateIfNotExists();
        }

        public async Task<User> GetUserAsync(string email)
        {
            var user = await _tableClient.QueryAsync<EsrTestHarnessUser>(t => t.PartitionKey == PartitionKey && t.RowKey == email.ToLower()).FirstOrDefaultAsync();

            return user != null ? new User { Email = user.RowKey, SignalRHubConnectionId = user.SignalRHubConnectionId } : null;
        }

        public async Task InsertUserAsync(User user)
        {
            await Upsert(user.Email, user.SignalRHubConnectionId);
        }

        public async Task UpdateUserAsync(string email, string signalRHubConnectionId)
        {
            await Upsert(email, signalRHubConnectionId);
        }

        private async Task Upsert(string email, string signalRHubConnectionId)
        {
            await _tableClient.UpsertEntityAsync(new EsrTestHarnessUser
            {
                PartitionKey = PartitionKey,
                RowKey = email.ToLower(),
                SignalRHubConnectionId = signalRHubConnectionId
            }, TableUpdateMode.Replace);
        }

        private class EsrTestHarnessUser : ITableEntity
        {
            public string SignalRHubConnectionId { get; set; }
            public string PartitionKey { get; set; }
            public string RowKey { get; set; }
            public DateTimeOffset? Timestamp { get; set; }
            public ETag ETag { get; set; }
        }
    }
}