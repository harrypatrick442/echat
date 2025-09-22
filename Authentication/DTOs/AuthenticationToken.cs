using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Authentication.DataMemberNames.Messages;


namespace Core.DTOs
{
    [DataContract]
    [KnownType(typeof(AuthenticationToken))]
    public class AuthenticationToken
    {
        private long _UserId;
        [JsonPropertyName(AuthenticationTokenDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = AuthenticationTokenDataMemberNames.UserId)]
        public long UserId { get { return _UserId; } set { _UserId = value; } }
        private string _Guid;
        [JsonPropertyName(AuthenticationTokenDataMemberNames.Guid)]
        [JsonInclude]
        [DataMember(Name = AuthenticationTokenDataMemberNames.Guid)]
        public string Guid { get { return _Guid; } set { _Guid = value; } }
        private long _ExpiresAt;
        [JsonPropertyName(AuthenticationTokenDataMemberNames.ExpiresAt)]
        [JsonInclude]
        [DataMember(Name = AuthenticationTokenDataMemberNames.ExpiresAt)]
        public long ExpiresAt { get { return _ExpiresAt; } protected set { _ExpiresAt = value; } }
        private long _CreatedAt;
        [JsonPropertyName(AuthenticationTokenDataMemberNames.CreatedAt)]
        [JsonInclude]
        [DataMember(Name = AuthenticationTokenDataMemberNames.CreatedAt)]
        public long CreatedAt { get { return _CreatedAt; } protected set { _CreatedAt = value; } }
        
        private string _DeviceIdentifier;
        [JsonPropertyName(AuthenticationTokenDataMemberNames.DeviceIdentifier)]
        [JsonInclude]
        [DataMember(Name = AuthenticationTokenDataMemberNames.DeviceIdentifier)]
        public string DeviceIdentifier { get { return _DeviceIdentifier; } protected set { _DeviceIdentifier = value; } }
        public AuthenticationToken(string guid, long userId, string deviceIdentifier, 
            long expiresAt, long createdAt) {
            _Guid = guid;
            _UserId = userId;
            _ExpiresAt = expiresAt;
            _CreatedAt = createdAt;
            _DeviceIdentifier = deviceIdentifier;
        }
        protected AuthenticationToken() { }
    }
}
