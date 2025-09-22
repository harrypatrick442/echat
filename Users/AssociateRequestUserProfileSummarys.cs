using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Users.DataMemberNames.Requests;

namespace Users
{
    [DataContract]
    public class AssociateRequestUserProfileSummarys
    {
        private AssociateRequestUserProfileSummary[] _Entries;
        [JsonPropertyName(AssociateRquestUserProfileSummarysDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name = AssociateRquestUserProfileSummarysDataMemberNames.Entries)]
        public AssociateRequestUserProfileSummary[] Entries
        {
            get { return _Entries; }
            protected set { _Entries = value; }
        }
        public AssociateRequestUserProfileSummarys(AssociateRequestUserProfileSummary[] entries)
        {
            _Entries = entries;
        }
        protected AssociateRequestUserProfileSummarys() { }

    }
}
