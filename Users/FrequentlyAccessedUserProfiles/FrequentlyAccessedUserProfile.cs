using Core.DataMemberNames;
using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Users.FrequentlyAccessedUserProfiles
{
    [DataContract]
    public class FrequentlyAccessedUserProfile
    {
        private string _Name;
        [JsonPropertyName(FrequentlyAccessedUserProfileDataMemberNames.Name)]
        [JsonInclude]
        [DataMember(Name = FrequentlyAccessedUserProfileDataMemberNames.Name)]
        public string Name
        {
            get { return _Name; }
            protected set { _Name = value; }
        }
        public FrequentlyAccessedUserProfile(string name)
        {
            _Name = name;
        }
        protected FrequentlyAccessedUserProfile() { }

    }
}
