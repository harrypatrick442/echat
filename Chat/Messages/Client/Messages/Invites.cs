using Chat.DataMemberNames.Messages;
using Chat.Interfaces;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UsersEnums;

namespace Chat.Messages.Client.Messages
{
    [DataContract]
    public class Invites
    {
        [JsonPropertyName(InvitesDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name = InvitesDataMemberNames.Entries)]
        public Invite[] Entries { get; protected set; }
        public void Add(long conversationId, long userId, long sentAt)
        {
            lock (this)
            {
                var invites = new Invite[] { new Invite(conversationId, userId, sentAt) };
                if (Entries == null)
                    Entries = invites;
                else
                    Entries = Entries
                        .Where(e=>e.ConversationId!=conversationId||e.UserId!=userId)
                        .Concat(invites)
                        .ToArray();
            }
        }
        public bool Remove(long conversationId, out long[] userIdsInvitingRemoved)
        {
            lock (this)
            {
                userIdsInvitingRemoved = null;
                if (Entries == null)
                    return false;
                List<Invite> toKeep = new List<Invite> ();
                HashSet<long> userIdsInvitingRemovedSet = null;
                //TODO Well aware many ways to improve performance of this subsystem but just not a priority atm.
                foreach (Invite entry in Entries)
                {
                    if (entry.ConversationId == conversationId) {
                        if (userIdsInvitingRemovedSet == null)
                            userIdsInvitingRemovedSet = new HashSet<long> { entry.UserId};
                        else
                            userIdsInvitingRemovedSet.Add(entry.UserId);
                        continue;
                    }
                    toKeep.Add(entry);
                }
                if (userIdsInvitingRemovedSet!=null)
                {
                    Entries = toKeep.ToArray();
                    userIdsInvitingRemoved = userIdsInvitingRemovedSet.ToArray();
                    return true;
                }
                return false;
            }
        }
        public bool Remove(long conversationId, long userIdInviting)
        {
            lock (this)
            {
                if (Entries == null)
                    return false;
                int length = Entries.Length;
                Entries = Entries.Where(e => e.ConversationId != conversationId||e.UserId!= userIdInviting).ToArray();
                return Entries.Length != length;
            }
        }
        public Invites()
        {

        }
    }
}
