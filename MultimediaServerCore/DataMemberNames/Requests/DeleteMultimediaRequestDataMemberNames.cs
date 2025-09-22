using MessageTypes.Attributes;
using MessageTypes.Internal;
namespace MultimediaServerCore.DataMemberNames.Requests
{
    [MessageType(InterserverMessageTypes.MultimediaDelete)]
    public class DeleteMultimediaRequestDataMemberNames
    {
        public const string
            RawMultimediaTokens = "m";
    }
}