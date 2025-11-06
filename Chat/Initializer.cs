using Core.DAL;
using MentionsCore;
using MultimediaServerCore;
using UserMultimediaCore;
namespace Chat
{
    public static class Initializer
    {
        public static void Initialize(bool isDebug) {
            int myNodeId = Nodes.Nodes.Instance.MyId;
            ConversationIdToNodeId.Initialize();
            ConversationIdSource.Initialize();
            MessageIdSource.Initialize();
            DalPms.Initialize();
            DalWalls.Initialize();
            DalComments.Initialize();
            DalLatestMessages.Initialize();
            DalConversationSnapshots.Initialize();
            DalChatRoomInfos.Initialize();
            DalUserRooms.Initialize();
            DalMessages.Initialize();
            DalInvites.Initialize();
            DalAbandonedRooms.Initialize();
            ChatRoomsMesh.Initialize();
            ChatManager.Initialize();
            ChatRooms.Initialize();
            MentionsMesh.Initialize();
            ConversationSnapshotsManager.Initialize();
            ChatMultimediaMesh.Initialize();
            ChatMultimediaEventListener.Initialize();
            bool isMostActiveChatroomsManager = myNodeId == (
                isDebug
                ? Configurations.Nodes.ECHAT_MOST_ACTIVE_ROOMS_MANAGER_DEBUG
                : Configurations.Nodes.ECHAT_MOST_ACTIVE_ROOMS_MANAGER
                );
            MostActiveChatRoomsWatcher.Initialize(isMostActiveChatroomsManager);
        }
    }
}