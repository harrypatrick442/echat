using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Users.DataMemberNames.Messages
{
    public static class UserProfileSummaryDataMemberNames
    {
        public const string UserId = "u", Username = "n", FirstName = "f", Surname = "s",
            MiddleNames = "m", AboutYou = "a", AssociateType = "t", MainPicture = "p",
            MainChildFriendlyPicture = "c";
    }
}
