using NodeAssignedIdRangesCore.DataMemberNames.Interserver.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NodeAssignedIdRanges
{
    [DataContract]
    [KnownType(typeof(IdRange))]
    public class IdRange
    {
        //CHECKED
        [JsonPropertyName(IdRangeDataMemberNames.FromInclusive)]
        [JsonInclude]
        [DataMember(Name = IdRangeDataMemberNames.FromInclusive)]
        public long FromInclusive { get; protected set; }
        [JsonPropertyName(IdRangeDataMemberNames.ToExclusive)]
        [JsonInclude]
        [DataMember(Name = IdRangeDataMemberNames.ToExclusive)]
        public long ToExclusive { get; protected set; }
        public IdRange(long fromInclusive, long toExclusive) {
            FromInclusive = fromInclusive;
            ToExclusive = toExclusive;
        }
        public IdRange() { 
            
        }
    }
}
