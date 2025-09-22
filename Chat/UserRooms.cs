using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Chat
{
    [DataContract]
    public class UserRooms
    {
        [JsonPropertyName(UserRoomsDataMemberNames.Pinned)]
        [JsonInclude]
        [DataMember(Name =UserRoomsDataMemberNames.Pinned, EmitDefaultValue =false)]
        public long[] Pinned { get; protected set; }
        [JsonPropertyName(UserRoomsDataMemberNames.Recent)]
        [JsonInclude]
        [DataMember(Name = UserRoomsDataMemberNames.Recent, EmitDefaultValue = false)]
        public List<long> Recent { get; protected set; }
        [JsonPropertyName(UserRoomsDataMemberNames.Mine)]
        [JsonInclude]
        [DataMember(Name = UserRoomsDataMemberNames.Mine, EmitDefaultValue = false)]
        public long[] Mine { get; protected set; }
        [JsonPropertyName(UserRoomsDataMemberNames.Joined)]
        [JsonInclude]
        [DataMember(Name = UserRoomsDataMemberNames.Joined, EmitDefaultValue = false)]
        public long[] Joined { get; protected set; }
        public UserRooms() { }
        public void Pin(long conversationId) {
            lock (this)
            {
                if (Pinned == null)
                {
                    Pinned = new long[] { conversationId };
                    return;
                }
                if (Pinned.Contains(conversationId)) return;
                Pinned = Pinned.Concat(new long[] { conversationId }).ToArray();
            }
        }
        public void Unpin(long conversationId)
        {
            lock (this)
            {
                Pinned = Pinned?.Where(c => c != conversationId).ToArray();
            }
        }
        public void AddRecent(long conversationId)
        {
            lock (this)
            {
                if (Recent == null)
                {
                    Recent = new List<long> { conversationId };
                    return;
                }
                Recent.Remove(conversationId);
                Recent.Add(conversationId);
                int nToRemove = Recent.Count - GlobalConstants.Lengths.MAX_RECENT_ROOMS;
                if (nToRemove <= 0) return;
                Recent.RemoveRange(0, nToRemove);
            }
        }
        public void RemoveRecent(long conversationId)
        {
            lock (this)
            {
                Recent.Remove(conversationId);
            }
        }
        public void AddMine(long conversationId)
        {
            lock (this)
            {
                if (Mine == null)
                {
                    Mine = new long[] { conversationId };
                    return;
                }
                if (Mine.Contains(conversationId)) return;
                Mine = Mine.Concat(new long[] { conversationId }).ToArray();
            }
        }
        public void RemoveMine(long conversationId)
        {
            lock (this)
            {
                Mine = Mine?.Where(c => c != conversationId).ToArray();
            }
        }
        public void AddJoined(long conversationId)
        {
            lock (this)
            {
                if (Joined == null)
                {
                    Joined = new long[] { conversationId };
                    return;
                }
                if (Joined.Contains(conversationId)) return;
                Joined = Joined.Concat(new long[] { conversationId }).ToArray();
            }
        }
        public void RemoveJoined(long conversationId)
        {
            lock (this)
            {
                Joined = Joined?.Where(c => c != conversationId).ToArray();
            }
        }
    }
}
