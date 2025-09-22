using Core.Enums;
using Core.Assets;
using Core.Exceptions;
using Chat;
using KeyValuePairDatabases.Appended;
using KeyValuePairDatabases;
using NodeAssignedIdRangesCore.Requests;
using NodeAssignedIdRanges;
using Chat.Interfaces;
using Chat.Messages.Client.Messages;

namespace Core.DAL
{
    public class DalMessagesAppendedJsonFiles
    {
        private NodesIdRangesForIdTypeManager _NodeIdRangesForMessagesManager;
        private static DalMessagesAppendedJsonFiles _Instance;
        public static DalMessagesAppendedJsonFiles Instance
        {
            get
            {
                if (_Instance == null)
                    throw new NotInitializedException(nameof(DalMessagesAppendedJsonFiles));
                return _Instance;
            }
        }
        public static DalMessagesAppendedJsonFiles Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalMessagesAppendedJsonFiles));
            _Instance = new DalMessagesAppendedJsonFiles();
            return _Instance;
        }

        private AppendedKeyValuePairOnDiskDatabase<ClientMessage>
            _MessagesAppendedKeyValuePairOnDiskDatabase;
        private DalMessagesAppendedJsonFiles()
        {
            _MessagesAppendedKeyValuePairOnDiskDatabase
            = new AppendedKeyValuePairOnDiskDatabase<ClientMessage>(
                Paths.MessagesDatabaseDirectory,
                nCharactersEachLevel: 2,
                MessageJsonParser.Instance,
                throwOnFailParseJsonObject: false,
                ConversationIdToNodeId.Instance,
                DatabaseIdentifier.Messages.Int(),
                new IdentifierLock<long>());
        }
        public long Append(long conversationId, ClientMessage message)
        {
            _MessagesAppendedKeyValuePairOnDiskDatabase.Append(conversationId, message, out long indexToContinueFromToGoBackFromMessage);
            return indexToContinueFromToGoBackFromMessage;
        }
        public ClientMessage[] ReadFromEnd(long conversationId, int nMessages, out long indexToContinueFrom)
        {
            return _MessagesAppendedKeyValuePairOnDiskDatabase.ReadBackwardsFromEnd(conversationId, nMessages, out indexToContinueFrom);
        }
        public ClientMessage[] ContinueRead(long conversationId, int nMessages,
            long? indexFrom, out long indexToContinueFrom)
        {
            return _MessagesAppendedKeyValuePairOnDiskDatabase.ContinueReadBackwards(
                conversationId, indexFrom, nMessages, out indexToContinueFrom);
        }
    }
}