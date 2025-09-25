using Core.Messages.Messages;
using LocationCore;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserLocation.DataMemberNames.Requests;

namespace UserLocation
{
    [DataContract]
    public class UserQuadTreeGetNEntriesRequest : TicketedMessageBase
    {
        [JsonPropertyName(UserQuadTreeGetNEntriesRequestDataMemberNames.Level)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeGetNEntriesRequestDataMemberNames.Level)]
        public int Level { get; protected set; }
        [JsonPropertyName(UserQuadTreeGetNEntriesRequestDataMemberNames.Quadrants)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeGetNEntriesRequestDataMemberNames.Quadrants)]
        public long[] Quadrants { get; protected set; }
        [JsonPropertyName(UserQuadTreeGetNEntriesRequestDataMemberNames.WithLatLng)]
        [JsonInclude]
        [DataMember(Name = UserQuadTreeGetNEntriesRequestDataMemberNames.WithLatLng)]
        public bool WithLatLng { get; protected set; }
        public UserQuadTreeGetNEntriesRequest(int level, long[] quadrants, bool withLatLng)
            : base(MessageTypes.UserQuadTreeGetNEntries)
        {
            Level = level;
            Quadrants = quadrants;
            WithLatLng = withLatLng;
        }
        protected UserQuadTreeGetNEntriesRequest()
            : base(MessageTypes.UserQuadTreeGetNEntries) { }
    }
}
