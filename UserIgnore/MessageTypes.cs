using Core.DataMemberNames;
using MessageTypes.Internal;
using System.Net.NetworkInformation;

namespace UserIgnore
{
    public class MessageTypes
    {
        public const string 
            UserIgnoreIgnore = "uii",
            UserIgnoreUnignore = "uiu",
            UserIgnoreGet = InterserverMessageTypes.UserIgnoresGet,
            UserIgnoreUnignored = "uiud",
            UserIgnoreIgnored = "uiid";
    }
}