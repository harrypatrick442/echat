using LocationCore.DataMemberNames.Messages;
using MessageTypes.Attributes;
using MultimediaCore.DataMemberNames.Messages;

namespace Users.DataMemberNames.Messages
{
    public class UserProfileDataMemberNames
    {
        public const string UserId = "i",
            Guest = "g",
            Title = "t",
            TitleVisibleTo = "tv",
            FirstName = "f",
            FirstNameVisibleTo = "fv",
            Username = "u",
            MiddleNames = "m",
            MiddleNamesVisibleTo = "mv",
            Surname = "s",
            SurnameVisibleTo = "sv",
            AboutYou = "a",
            AboutYouVisibleTo = "av",
            MobilePhoneNumber = "mp",
            MobilePhoneNumberVisibleTo = "mpv",
            WorkPhoneNumber = "wp",
            WorkPhoneNumberVisibleTo = "wpv",
            ContactEmail = "ce",
            ContactEmailVisibleTo = "cev",
            WorkEmail = "we",
            WorkEmailVisibleTo = "wev",
            Gender = "ge",
            GenderVisibleTo = "gv",
            Birthday = "b",
            BirthdayVisibleTo = "bv",
            LocationVisibleTo = "lv",
            WallConversationId = "wc";
        [DataMemberNamesClass(typeof(AbstractLocationDataMemberNames), isArray: false)]
        public const string Location = "l";
        [DataMemberNamesClass(typeof(UserMultimediaItemDataMemberNames), isArray: true)]
        public const string Pictures = "p";
        [DataMemberNamesClass(typeof(UserMultimediaItemDataMemberNames), isArray: true)]
        public const string Videos = "v";
    }
}
