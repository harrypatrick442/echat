using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Messages;
namespace UserIgnore
{
    [DataContract]
    public class BeingIgnoredBys : IgnoreEntriesBase
    {
        [JsonPropertyName(BeingIgnoredBysDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name =BeingIgnoredBysDataMemberNames.Entries)]
        public long[] Entries
        {
            get
            {
                lock (this)
                {
                    return _Entries;
                }
            }
            protected set
            {
                lock (this)
                {
                    _Entries = value;
                }
            }
        }
        public BeingIgnoredBys() {
            
        }
    }
}
