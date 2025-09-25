using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Messages;
namespace UserIgnore.Messages
{
    [DataContract]
    public class IgnoredUser : TypedMessageBase
    {
        [JsonPropertyName(IgnoredUserDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = IgnoredUserDataMemberNames.UserId)]
        public long UserId { get; protected set; }
        public IgnoredUser(long userId)
            : base()
        {
            UserId = userId;
            Type = MessageTypes.UserIgnoreIgnored;
        }
        protected IgnoredUser() { }
    }
}
