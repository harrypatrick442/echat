using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Chat.Messages.Client.Messages;
using Core.FileSystem;
using JSON;

namespace Chat
{
    public class LatestMessages
    {
        private int _NMessages;
        private List<ClientMessage> _Messages;
        private List<ClientMessage> _MessagesToFlushToCyclicalFile = new List<ClientMessage>();
        private CyclicalFile _CyclicalFile;
        public LatestMessages(int nMessages, string cyclicalFilePath)
        {
            _NMessages = nMessages;
            _CyclicalFile = new CyclicalFile(cyclicalFilePath, 
                entrySize: GlobalConstants.Sizes.MAX_OVERFLOWING_CHAT_MESSAGE_SIZE, nMessages);
            _Messages = _ReadCyclicalFile();
        }
        public ClientMessage[] AllMessages { get { lock (_Messages) { return _Messages.ToArray(); } } }
        public void Append(ClientMessage message)
        {
            lock (_Messages)
            {
                _MessagesToFlushToCyclicalFile.Add(message);
                _OverflowLinkedList(_MessagesToFlushToCyclicalFile);
                _Messages.Add(message);
                _OverflowLinkedList(_Messages);
            }
        }
        public void FlushToCyclicalFile() {
            ClientMessage[] messagesToFlushToCyclicalFile;
            lock (_Messages)
            {
                messagesToFlushToCyclicalFile = _MessagesToFlushToCyclicalFile.ToArray();
                _MessagesToFlushToCyclicalFile.Clear();
            }
            List<byte[]> bytess = messagesToFlushToCyclicalFile
                .Select(_GetMessageBytesWithNullTerminator).ToList();
            _CyclicalFile.Write(bytess);
        }
        private List<ClientMessage> _ReadCyclicalFile() {
            byte[][] bytess = _CyclicalFile.Read();
            if (bytess == null || bytess.Length < 1)
                return new List<ClientMessage>();
            List<ClientMessage> messages = new List<ClientMessage>();
            foreach (byte[] bytes in bytess) {
                ClientMessage message = _ParseMessageFromBytes(bytes);
                if (message == null) continue;
                messages.Add(message);
            }
            return messages;
        }
        private ClientMessage _ParseMessageFromBytes(byte[] bytes) {
            int indexOfNull = 0;
            while (indexOfNull < bytes.Length) {
                if (bytes[indexOfNull] == 0) break;
                indexOfNull++;
            }
            string jsonString = System.Text.Encoding.UTF8.GetString(bytes, 0, indexOfNull);
            return Json.Deserialize<ClientMessage>(jsonString);
        }
        private byte[] _GetMessageBytesWithNullTerminator(ClientMessage message) {
            string jsonString = Json.Serialize(message);
            byte[] bytes =  System.Text.Encoding.UTF8.GetBytes(jsonString);
            if (bytes.Length > GlobalConstants.Sizes.MAX_OVERFLOWING_CHAT_MESSAGE_SIZE)
                throw new InvalidDataContractException($"Exceeded {nameof(GlobalConstants.Sizes.MAX_OVERFLOWING_CHAT_MESSAGE_SIZE)}");
            return bytes;
        }
        private void _OverflowLinkedList(List<ClientMessage> linkedList) {
            int nToRemove = linkedList.Count - _NMessages;
            if (nToRemove < 1) return;
            linkedList.RemoveRange(0, nToRemove);
        }
    }
}
