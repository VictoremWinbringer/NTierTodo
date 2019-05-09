namespace NTierTodo.Dal
{
    public interface IMessageNotifier
    {
        void NotifyAll(string message);
    }
}