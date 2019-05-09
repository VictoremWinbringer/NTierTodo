using Microsoft.AspNetCore.SignalR;
using NTierTodo.SignalR;

namespace NTierTodo.Dal
{
    public class SignalRMessageNotifier: IMessageNotifier
    {
        private readonly IHubContext<Notifier> _notifier;

        public SignalRMessageNotifier(IHubContext<Notifier> notifier)
        {
            _notifier = notifier;
        }
        
        public void NotifyAll(string message)
        {
            _notifier.Clients.All.InvokeAsync("Notify", message);
        }
    }
}