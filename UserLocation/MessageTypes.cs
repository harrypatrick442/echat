using Core.DataMemberNames;
using MessageTypes.Internal;
using System.Net.NetworkInformation;

namespace UserLocation
{
    public class MessageTypes
    {
        public const string
        UserQuadTreeGet = "uqtg",
        UserQuadTreeSet = "uqts",
        UserQuadTreeGetNEntries = "uqtgne";
    }
}