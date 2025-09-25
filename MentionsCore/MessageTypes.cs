using Core.DataMemberNames;
using MessageTypes.Internal;
using System.Net.NetworkInformation;

namespace MentionsCore
{
    public class MessageTypes
    {
        public const string
        MentionsMention = "mem",
        MentionsGet = InterserverMessageTypes.MentionsGet,
        MentionsAddOrUpdate = InterserverMessageTypes.MentionsAddOrUpdate,
        MentionsSetSeen = InterserverMessageTypes.MentionsSetSeen;
    }
}