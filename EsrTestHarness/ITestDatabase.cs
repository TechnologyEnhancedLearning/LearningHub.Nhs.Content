using EsrTestHarness.Model;
using System.Threading.Tasks;

namespace EsrTestHarness
{
    public interface ITestDatabase
    {
        Task<User> GetUserAsync(string email);

        Task InsertUserAsync(User user);

        Task UpdateUserAsync(string email, string signalRHubConnectionId);
    }
}