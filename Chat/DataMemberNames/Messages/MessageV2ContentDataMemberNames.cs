using MessageTypes.Attributes;

namespace Chat.DataMemberNames.Messages
{
    public static class MessageV2ContentDataMemberNames
    {
        [DataMemberNamesClass(typeof(MessageV2ContentEntryDataMemberNames), isArray: true)]
        public const string Entries = "e";
    }
}