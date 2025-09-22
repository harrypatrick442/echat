using Chat.Messages.Client.Messages;
using JSON;

namespace Chat
{
    public class MessageJsonParser:IJsonParser<ClientMessage>
    {
        private static readonly MessageJsonParser _Instance = new MessageJsonParser();
        public static MessageJsonParser Instance { get { return _Instance; } }
        public ClientMessage Deserialize(string jsonString)
        {
            if (jsonString == null || jsonString.Length < 1) return null;
            bool deleted = jsonString[0] == 'D';
            if (deleted)
            {
                jsonString= '{' + jsonString.Substring(1, jsonString.Length - 1);
            }
            ClientMessage message = Json.Deserialize<ClientMessage>(jsonString);
            message.Deleted = deleted;
            return message;
        }

        public string Serialize(ClientMessage item, bool prettify = false)
        {
            string jsonString = Json.Serialize(item, prettify);
            if (item.Deleted) { 
                jsonString = 'D' + jsonString.Substring(1, jsonString.Length - 1);
            }
            return jsonString;
        }
    }
}