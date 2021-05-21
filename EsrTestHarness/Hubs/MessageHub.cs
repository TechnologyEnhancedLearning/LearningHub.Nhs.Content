using Microsoft.AspNetCore.SignalR;

namespace EsrTestHarness.Hubs
{
    public class MessageHub : Hub
    {
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}