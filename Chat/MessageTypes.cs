using Core.DataMemberNames;
using MessageTypes.Internal;
using System.Net.NetworkInformation;

namespace Chat
{
    public class MessageTypes
    {
        public const string
        ChatSearchRooms = "csr",
        ChatBanUser = "cbu",
        ChatUnbanUser = "cubu",
        ChatFetchIndividualMessages = InterserverMessageTypes.ChatFetchIndividualMessages,
        ChatGetAdministrators = InterserverMessageTypes.ChatGetAdministrators,
        ChatSetAdministrator = InterserverMessageTypes.ChatSetAdministrator,
        ChatRemoveAdministrator = InterserverMessageTypes.ChatRemoveAdministrator,
        ChatRemoveRoomInvite = "crsri",
        ChatAddRoomInvite = "ciari",
        ChatLeaveRoom = InterserverMessageTypes.ChatLeaveRoom,
        ChatGetMyReceivedInvites = InterserverMessageTypes.ChatGetMyReceivedInvites,
        ChatGetMySentInvites = InterserverMessageTypes.ChatGetMySentInvites,
        ChatRoomInvite = InterserverMessageTypes.ChatRoomInvite,
        ChatAcceptRoomInvite = InterserverMessageTypes.ChatAcceptRoomInvite,
        ChatRejectRoomInvite = InterserverMessageTypes.ChatRejectRoomInvite,
        ChatCancelRoomInvite = InterserverMessageTypes.ChatCancelRoomInvite,
        ChatGetWallConversation = InterserverMessageTypes.ChatGetWallConversation,
            ChatGetWallCommentsConversation = InterserverMessageTypes.ChatGetWallCommentsConversation,
        ChatIncorrectPassword = "cip",
        ChatFailedEnterRoom = "cfer",
        ChatAttemptEnterRoom = "caer",
        ChatEnteredRoom = "cer",
        ChatRoomUserCameOnline = "cruo",
        ChatRoomUserWentOffline = "crup",
        ChatRoomUserJoined = "cruj",
        ChatRoomUserLeft = "crul",
        ClientMessage = InterserverMessageTypes.ClientMessage,
        ChatRoomSendMessage = "crsm",
        ChatGetConversationSnapshots = "cgmcs",
        ChatUploadRoomPicture = "curp",
        ChatUploadMessagePicture = "cump",
        ChatPmUploadMessagePicture = "cpump",
        ChatUploadMessageVideo = "cumv",
        ChatPmUploadMessageVideo = "cpumv",
        ChatGetPmConversationWithLatestMessages = "cpg",
        ChatUpdateRoomInfo = "curi",
        ChatLoadMessagesHistory = InterserverMessageTypes.ChatLoadMessagesHistory,
        ChatLoadMessagesHistoryBySentAt = InterserverMessageTypes.ChatLoadMessagesHistoryBySentAt,
        ChatModifyMessage = InterserverMessageTypes.ChatModifyMessage,
        ChatDeleteMessages = InterserverMessageTypes.ChatDeleteMessages,
        ChatReactToMessage = InterserverMessageTypes.ChatReactToMessage,
        ChatUnreactToMessage = InterserverMessageTypes.ChatUnreactToMessage,
        ChatGetUserRooms = InterserverMessageTypes.ChatGetUserRooms,
        ChatModifyUserRooms = InterserverMessageTypes.ChatModifyUserRooms,
        ChatGetRoomSummarys = InterserverMessageTypes.ChatGetRoomSummarys,
        ChatMultimediaUpload = InterserverMessageTypes.ChatMultimediaUpload,
        ChatCreateRoom = InterserverMessageTypes.ChatCreateRoom,
        ChatRemoveUserFromActiveConversations = InterserverMessageTypes.ChatRemoveUserFromActiveConversations,
        ChatSetSeenMessage = InterserverMessageTypes.ChatSetSeenMessage;
    }
}