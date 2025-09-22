namespace Chat
{
    public class ConversationSnapshotInLinkedList
    {
        private ConversationSnapshot ConversationSnapshot { get; }
        public LinkedListNode<ConversationSnapshotInLinkedList>  LinkedListNode{ get; set;}
        public ConversationSnapshotInLinkedList(ConversationSnapshot conversationSnapshot) {
            ConversationSnapshot = conversationSnapshot;
        }
    }
}