
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.DataMemberNames;
using Users.FrequentlyAccessedUserProfiles;
using MessageTypes.Internal;

namespace Users.Messages.Client
{
    [DataContract]
    public class FrequentlyAccessedUserProfileUpdateRequest
    {
        [JsonPropertyName(MessageTypeDataMemberName.Value)]
        [JsonInclude]
        [DataMember(Name = MessageTypeDataMemberName.Value)]
        public string Type
        {
            get { return InterserverMessageTypes.UserFrequentlyAccessedUserProfileUpdate; }
            protected set { }
        }
        private long _UserId;
        [JsonPropertyName(FrequentlyAccessedUserProfileUpdateRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = FrequentlyAccessedUserProfileUpdateRequestDataMemberNames.UserId)]

        public long UserId
        {
            get { return _UserId; }
            protected set { _UserId = value; }
        }
        private FrequentlyAccessedUserProfile _FrequentlyAccessedUserProfile;
        [JsonPropertyName(FrequentlyAccessedUserProfileUpdateRequestDataMemberNames.FrequentlyAccessedUserProfile)]
        [JsonInclude]
        [DataMember(Name = FrequentlyAccessedUserProfileUpdateRequestDataMemberNames.FrequentlyAccessedUserProfile)]

        public FrequentlyAccessedUserProfile FrequentlyAccessedUserProfile { 
            get { return _FrequentlyAccessedUserProfile; } 
            protected set { _FrequentlyAccessedUserProfile = value; } }
        public FrequentlyAccessedUserProfileUpdateRequest(long userId,
            FrequentlyAccessedUserProfile frequentlyAccessedUserProfile) {
            _UserId = userId;
            _FrequentlyAccessedUserProfile = frequentlyAccessedUserProfile;
        }
        protected FrequentlyAccessedUserProfileUpdateRequest() { }
    }
}
