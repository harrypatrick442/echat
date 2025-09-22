
namespace Chat
{
    public enum ChatFailedReason
    {
        None=0,
        ConversationDoesNotExist = 1,
        UserNotIncluded=3,
        ServerError = 4,
        RateLimited = 5
    }
}