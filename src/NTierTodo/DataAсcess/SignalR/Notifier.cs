using Microsoft.AspNetCore.SignalR;

namespace NTierTodo.SignalR
{
    public class Notifier : Hub
    {
        public void Notify(string message)
        {
            Clients.All.InvokeAsync("Notify", message);
        }
    }
}
