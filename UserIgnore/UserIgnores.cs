using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using UserIgnore.DataMemberNames.Messages;
namespace UserIgnore
{
    [DataContract]
    public class UserIgnores: IgnoreEntriesBase
    {
        [JsonPropertyName(UserIgnoresDataMemberNames.Entries)]
        [JsonInclude]
        [DataMember(Name = UserIgnoresDataMemberNames.Entries)]
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
        public UserIgnores() {

        }
    }
}
