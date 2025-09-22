using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace NodeAssignedIdRangesSource.Serializables
{
    //CHECKED
    [DataContract]
    public class NextIdFromForIdType
    {
        [JsonPropertyName(NextIdFromForIdTypeDataMemberNames.Value)]
        [JsonInclude]
        [DataMember(Name = NextIdFromForIdTypeDataMemberNames.Value)]
        public long Value { get; set; }
        public NextIdFromForIdType(long value)
        {
            Value = value;
        }
        protected NextIdFromForIdType() { }
    }
}
