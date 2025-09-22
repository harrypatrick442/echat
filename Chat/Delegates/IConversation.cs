namespace Core.Chat
{
    public interface IConversation
    {
        void UsingUserIds(Action<IEnumerable<long>> callback);
        long[] UserIdsToArray();
    }
}
