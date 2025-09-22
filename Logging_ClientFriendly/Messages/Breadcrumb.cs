using Core.Enums;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Strings;
using Logging;
using Logging_ClientFriendly.DataMemberNames.Messages;

namespace Logging_ClientFriendly.Messages
{
    [DataContract]
    public class Breadcrumb : IBreadcrumb
    {
        [JsonPropertyName(BreadcrumbDataMemberNames.Id)]
        [JsonInclude]
        [DataMember(Name = BreadcrumbDataMemberNames.Id)]
        public long Id { get; set; }
        [JsonPropertyName(BreadcrumbDataMemberNames.SessionId)]
        [JsonInclude]
        [DataMember(Name = BreadcrumbDataMemberNames.SessionId)]
        public long SessionId { get; protected set; }
        [JsonPropertyName(BreadcrumbDataMemberNames.AtClientUTC)]
        [JsonInclude]
        [DataMember(Name = BreadcrumbDataMemberNames.AtClientUTC)]
        public long AtClientUTC { get; protected set; }
        [JsonPropertyName(BreadcrumbDataMemberNames.AtServerUTC)]
        [JsonInclude]
        [DataMember(Name = BreadcrumbDataMemberNames.AtServerUTC)]
        public long AtServerUTC { get; set; }
        [JsonPropertyName(BreadcrumbDataMemberNames.TypeId)]
        [JsonInclude]
        [DataMember(Name = BreadcrumbDataMemberNames.TypeId)]
        public int TypeId { get; set; }
        public BreadcrumbType BreadcrumbType { get { return BreadcrumbTypeHelper.Parse(TypeId); } }
        [JsonPropertyName(BreadcrumbDataMemberNames.Description)]
        [JsonInclude]
        [DataMember(Name = BreadcrumbDataMemberNames.Description)]
        public string Description { get; set; }
        [JsonPropertyName(BreadcrumbDataMemberNames.Value)]
        [JsonInclude]
        [DataMember(Name = BreadcrumbDataMemberNames.Value)]
        public string Value { get; set; }
        [JsonPropertyName(BreadcrumbDataMemberNames.ValueHash)]
        [JsonInclude]
        [DataMember(Name = BreadcrumbDataMemberNames.ValueHash)]
        public long ValueHash { get; protected set; }
        public void CalculateHashes()
        {
            ValueHash = Value == null ? 0 : Value.ToNonCryptographicHash();
        }
        public Breadcrumb(long sessionId, long atClientUTC, BreadcrumbType type, string description, string value)
        {
            SessionId = sessionId;
            AtClientUTC = atClientUTC;
            TypeId = (int)type;
            Description = description;
            Value = value;
        }
        public Breadcrumb(long id, long sessionId, long atClientUTC, BreadcrumbType type, string description,
            string value) : this(sessionId, atClientUTC, type, description, value)
        {
            Id = id;
        }
        protected Breadcrumb() { }

    }
}
