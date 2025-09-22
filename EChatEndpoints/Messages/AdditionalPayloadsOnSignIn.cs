using EChat.DataMemberNames.Messages;
using MentionsCore.DataMemberNames.Messages;
using MessageTypes.Attributes;
using MessageTypes.Internal;
using NotificationsCore.DataMemberNames.Messages;
using NotificationsCore.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users;
using Users.Messages.Client;

namespace EChat.Messages
{
    public class AdditionalPayloadsOnSignIn
    {

        [JsonPropertyName(AdditionalPayloadsOnSignInDataMemberNames.UserNotifications)]
        [JsonInclude]
        [DataMember(Name = AdditionalPayloadsOnSignInDataMemberNames.UserNotifications)]
        public UserNotifications? UserNotifications { get; protected set; }
        [JsonPropertyName(AdditionalPayloadsOnSignInDataMemberNames.AllAssociateEntries)]
        [JsonInclude]
        [DataMember(Name = AdditionalPayloadsOnSignInDataMemberNames.AllAssociateEntries)]
        public GetAllAssociateEntriesResponse AllAssociateEntries { get; protected set; }
        public AdditionalPayloadsOnSignIn(
                UserNotifications? userNotifications,
                GetAllAssociateEntriesResponse allAssociateEntries)
        {
            UserNotifications = userNotifications;
            AllAssociateEntries = allAssociateEntries;
        }
    }
}