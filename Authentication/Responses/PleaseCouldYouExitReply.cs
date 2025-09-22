using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Authentication.Enums;

namespace Authentication.Responses
{
    [DataContract]
    public class PleaseCouldYouExitReply
    {
        private PleaseCouldYouExitRsponse _Response;
        [JsonPropertyName("response")]
        [JsonInclude]
        [DataMember(Name = "response")]
        public PleaseCouldYouExitRsponse Response { get { return _Response; } protected set { _Response = value; } }
        protected PleaseCouldYouExitReply()
        {

        }
        public PleaseCouldYouExitReply(PleaseCouldYouExitRsponse response)
        {
            _Response = response;
        }
    }
}
