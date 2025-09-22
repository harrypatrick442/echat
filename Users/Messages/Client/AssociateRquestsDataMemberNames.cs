using MessageTypes.Attributes;

namespace Users.DataMemberNames.Messages
{
    public static class AssociateRquestsDataMemberNames
    {
        [DataMemberNamesClass(typeof(AssociateRquestDataMemberNames), isArray: true)]
        public const string Entries = "e";

    }
}