using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Authentication.Enums
{
    [DataContract]
    public enum PleaseCouldYouExitRsponse
    {
        YesICan,
        SorryImBusyPleaseWait
    }
}
