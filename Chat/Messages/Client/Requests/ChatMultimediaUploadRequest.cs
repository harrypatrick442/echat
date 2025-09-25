using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Core.Messages.Messages;
using UsersEnums;
using MultimediaServerCore;
using FileInfo = Core.Messages.Messages.FileInfo;
using Chat.DataMemberNames.Requests;
namespace Chat.Messages.Client.Requests
{
    [DataContract]
    public class ChatMultimediaUploadRequest : TicketedMessageBase
    {
        [JsonPropertyName(ChatMultimediaUploadRequestDataMemberNames.MultimediaType)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadRequestDataMemberNames.MultimediaType)]
        public MultimediaType MultimediaType
        {
            get;
            protected set;
        }
        [JsonPropertyName(ChatMultimediaUploadRequestDataMemberNames.ScopeType)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadRequestDataMemberNames.ScopeType)]
        public MultimediaScopeType ScopeType
        {
            get;
            protected set;
        }
        [JsonPropertyName(ChatMultimediaUploadRequestDataMemberNames.ConversationId)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadRequestDataMemberNames.ConversationId)]
        public long ConversationId
        {
            get;
            protected set;
        }
        [JsonPropertyName(ChatMultimediaUploadRequestDataMemberNames.ConversationType)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadRequestDataMemberNames.ConversationType)]
        public ConversationType ConversationType
        {
            get;
            protected set;
        }
        [JsonPropertyName(ChatMultimediaUploadRequestDataMemberNames.FileInfo)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadRequestDataMemberNames.FileInfo)]
        public FileInfo FileInfo
        {
            get;
            protected set;
        }
        [JsonPropertyName(ChatMultimediaUploadRequestDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadRequestDataMemberNames.UserId)]
        public long UserId
        {
            get;
            protected set;
        }
        [JsonPropertyName(ChatMultimediaUploadRequestDataMemberNames.SessionId)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadRequestDataMemberNames.SessionId)]
        public long? SessionId
        {
            get;
            protected set;
        }
        [JsonPropertyName(ChatMultimediaUploadRequestDataMemberNames.XRating)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadRequestDataMemberNames.XRating)]
        public XRating XRating
        {
            get;
            protected set;
        }
        [JsonPropertyName(ChatMultimediaUploadRequestDataMemberNames.Description)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadRequestDataMemberNames.Description)]
        public string Description
        {
            get;
            protected set;
        }
        [JsonPropertyName(ChatMultimediaUploadRequestDataMemberNames.AlreadyCheckedPermission)]
        [JsonInclude]
        [DataMember(Name = ChatMultimediaUploadRequestDataMemberNames.AlreadyCheckedPermission)]
        public bool AlreadyCheckedPermission
        {
            get;
            protected set;
        }
        public ChatMultimediaUploadRequest(MultimediaType multimediaType, 
            MultimediaScopeType scopeType,  long conversationId, ConversationType conversationType,
            FileInfo fileInfo, long userId, long? sessionId, XRating xRating, string description, 
            bool alreadyCheckedPermission)
            : base(MessageTypes.ChatMultimediaUpload)
        {
            MultimediaType = multimediaType;
            ConversationId = conversationId;
            ConversationType = conversationType;
            FileInfo = fileInfo;
            UserId = userId;
            SessionId = sessionId;
            XRating = xRating;
            Description = description;
            AlreadyCheckedPermission = alreadyCheckedPermission;
        }
        protected ChatMultimediaUploadRequest()
            : base(MessageTypes.ChatMultimediaUpload) { }
    }
}
