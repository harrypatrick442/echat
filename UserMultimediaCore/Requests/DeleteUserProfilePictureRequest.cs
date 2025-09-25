using Core.Messages.Messages;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using FileInfo = Core.Messages.Messages.FileInfo;
using UsersEnums;
using UserMultimediaCore.DataMemberNames.Requests;

namespace UserMultimediaCore.Requests
{
    [DataContract]
    public class DeleteUserProfilePictureRequest : TicketedMessageBase
    {
        [JsonPropertyName(DeleteUserProfilePictureRequestDataMemberNames.MultimediaToken)]
        [JsonInclude]
        [DataMember(Name = DeleteUserProfilePictureRequestDataMemberNames.MultimediaToken)]
        public string MultimediaToken { get; protected set; }
        public DeleteUserProfilePictureRequest(string multimediaToken) : 
            base(MessageTypes.MultimediaDeleteProfilePicture)
        {
            MultimediaToken = multimediaToken;
        }
        protected DeleteUserProfilePictureRequest() :
            base(MessageTypes.MultimediaDeleteProfilePicture)
        { }
    }
}
