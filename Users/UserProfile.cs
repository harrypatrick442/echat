using Users.FrequentlyAccessedUserProfiles;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Text;
using Users.Enums;
using UsersEnums;
using LocationCore;
using MultimediaCore;
using Users.DataMemberNames.Messages;

namespace Users
{
    [DataContract]
    public class UserProfile
    {
        private long _UserId;
        [JsonPropertyName(UserProfileDataMemberNames.UserId)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.UserId)]
        public long UserId { get { return _UserId; } protected set { _UserId = value; } }


        [JsonPropertyName(UserProfileDataMemberNames.WallConversationId)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.WallConversationId)]
        public long WallConversationId{ get; set; }


        [JsonPropertyName(UserProfileDataMemberNames.Guest)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.Guest)]
        public bool Guest { get; protected set; }
        private string _Username;
        private bool _UsernameSet;
        [JsonPropertyName(UserProfileDataMemberNames.Username)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.Username)]
        public string Username { 
            get { return _Username; } 
            protected set { 
                _Username = value;
                _UsernameSet = true;
            }
        }
        private bool _TitleSet;
        private Title _Title;
        [JsonPropertyName(UserProfileDataMemberNames.Title)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.Title)]
        public Title Title {
            get { return _Title; }
            protected set { _Title = value; _TitleSet = true; }
        }
        private bool _TitleVisibleToSet;
        private VisibleTo _TitleVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.TitleVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.TitleVisibleTo)]
        public VisibleTo TitleVisibleTo
        {
            get { return _TitleVisibleTo; }
            set { _TitleVisibleTo = value; _TitleVisibleToSet = true; }
        }
        private bool _FirstNameSet;
        private string _FirstName;
        [JsonPropertyName(UserProfileDataMemberNames.FirstName)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.FirstName)]
        public string FirstName
        {
            get { return _FirstName; }
            protected set { _FirstName = value; _FirstNameSet = true; }
        }
        private bool _FirstNameVisibleToSet;
        private VisibleTo _FirstNameVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.FirstNameVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.FirstNameVisibleTo)]
        public VisibleTo FirstNameVisibleTo
        {
            get { return _FirstNameVisibleTo; }
            set { _FirstNameVisibleTo = value; _FirstNameVisibleToSet = true; }
        }
        private bool _MiddleNamesSet;
        private string[] _MiddleNames;
        [JsonPropertyName(UserProfileDataMemberNames.MiddleNames)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.MiddleNames)]
        public string[] MiddleNames
        {
            get { return _MiddleNames; }
            protected set { _MiddleNames = value; _MiddleNamesSet = true; }
        }
        private bool _MiddleNamesVisibleToSet;
        private VisibleTo _MiddleNamesVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.MiddleNamesVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.MiddleNamesVisibleTo)]
        public VisibleTo MiddleNamesVisibleTo
        {
            get { return _MiddleNamesVisibleTo; }
            set { _MiddleNamesVisibleTo = value; _MiddleNamesVisibleToSet = true; }
        }
        private bool _SurnameSet;
        private string _Surname;
        [JsonPropertyName(UserProfileDataMemberNames.Surname)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.Surname)]
        public string Surname {
            get { return _Surname; }
            protected set { _Surname = value; _SurnameSet = true; }
        }
        private bool _SurnameVisibleToSet;
        private VisibleTo _SurnameVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.SurnameVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.SurnameVisibleTo)]
        public VisibleTo SurnameVisibleTo
        {
            get { return _SurnameVisibleTo; }
            set { _SurnameVisibleTo = value; _SurnameVisibleToSet = true; }
        }
        private bool _AboutYouSet;
        private string _AboutYou;
        [JsonPropertyName(UserProfileDataMemberNames.AboutYou)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.AboutYou)]
        public string AboutYou { 
            get { return _AboutYou; }
             set { _AboutYou = value; _AboutYouSet = true; }
        }
        private bool _AboutYouVisibleToSet;
        private VisibleTo _AboutYouVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.AboutYouVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.AboutYouVisibleTo)]
        public VisibleTo AboutYouVisibleTo
        {
            get { return _AboutYouVisibleTo; }
             set { _AboutYouVisibleTo = value; _AboutYouVisibleToSet = true; }
        }
        private bool _MobilePhoneNumberSet;
        private string _MobilePhoneNumber;
        [JsonPropertyName(UserProfileDataMemberNames.MobilePhoneNumber)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.MobilePhoneNumber)]
        public string MobilePhoneNumber
        {
            get { return _MobilePhoneNumber; }
             set { _MobilePhoneNumber = value; _MobilePhoneNumberSet = true; }
        }
        private bool _MobilePhoneNumberVisibleToSet;
        private VisibleTo _MobilePhoneNumberVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.MobilePhoneNumberVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.MobilePhoneNumberVisibleTo)]
        public VisibleTo MobilePhoneNumberVisibleTo
        {
            get { return _MobilePhoneNumberVisibleTo; }
             set { _MobilePhoneNumberVisibleTo = value; _MobilePhoneNumberVisibleToSet = true; }
        }
        private bool _WorkPhoneNumberSet;
        private string _WorkPhoneNumber;
        [JsonPropertyName(UserProfileDataMemberNames.WorkPhoneNumber)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.WorkPhoneNumber)]
        public string WorkPhoneNumber
        {
            get { return _WorkPhoneNumber; }
             set { _WorkPhoneNumber = value; _WorkPhoneNumberSet = true; }
        }
        private bool _WorkPhoneNumberVisibleToSet;
        private VisibleTo _WorkPhoneNumberVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.WorkPhoneNumberVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.WorkPhoneNumberVisibleTo)]
        public VisibleTo WorkPhoneNumberVisibleTo
        {
            get { return _WorkPhoneNumberVisibleTo; }
             set { _WorkPhoneNumberVisibleTo = value; _WorkPhoneNumberVisibleToSet = true; }
        }
        private bool _ContactEmailSet;
        private string _ContactEmail;
        [JsonPropertyName(UserProfileDataMemberNames.ContactEmail)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.ContactEmail)]
        public string ContactEmail
        {
            get { return _ContactEmail; }
            set { _ContactEmail = value; _ContactEmailSet = true; }
        }
        private bool _ContactEmailVisibleToSet;
        private VisibleTo _ContactEmailVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.ContactEmailVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.ContactEmailVisibleTo)]
        public VisibleTo ContactEmailVisibleTo
        {
            get { return _ContactEmailVisibleTo; }
             set { _ContactEmailVisibleTo = value; _ContactEmailVisibleToSet = true; }
        }
        private bool _WorkEmailSet;
        private string _WorkEmail;
        [JsonPropertyName(UserProfileDataMemberNames.WorkEmail)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.WorkEmail)]
        public string WorkEmail
        {
            get { return _WorkEmail; }
            set { _WorkEmail = value; _WorkEmailSet = true; }
        }
        private bool _WorkEmailVisibleToSet;
        private VisibleTo _WorkEmailVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.WorkEmailVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.WorkEmailVisibleTo)]
        public VisibleTo WorkEmailVisibleTo
        {
            get { return _WorkEmailVisibleTo; }
            set { _WorkEmailVisibleTo = value; _WorkEmailVisibleToSet = true; }
        }
        private bool _GenderSet;
        private Gender _Gender;
        [JsonPropertyName(UserProfileDataMemberNames.Gender)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.Gender)]
        public Gender Gender
        {
            get { return _Gender; }
             set { _Gender = value; _GenderSet = true; }
        }
        private bool _GenderVisibleToSet;
        private VisibleTo _GenderVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.GenderVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.GenderVisibleTo)]
        public VisibleTo GenderVisibleTo
        {
            get { return _GenderVisibleTo; }
             set { _GenderVisibleTo = value; _GenderVisibleToSet = true; }
        }
        private bool _BirthdaySet;
        private long _Birthday;
        [JsonPropertyName(UserProfileDataMemberNames.Birthday)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.Birthday)]
        public long Birthday { get { return _Birthday; }
             set { _Birthday = value; _BirthdaySet = true; }
        }
        private bool _BirthdayVisitleToSet;
        private VisibleTo _BirthdayVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.BirthdayVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.BirthdayVisibleTo)]
        public VisibleTo BirthdayVisibleTo
        {
            get { return _BirthdayVisibleTo; }
            set { _BirthdayVisibleTo = value; _BirthdayVisitleToSet = true; }
        }
        private bool _LocationSet;
        private AbstractLocation _Location;
        [JsonPropertyName(UserProfileDataMemberNames.Location)]
        [JsonInclude]
        [DataMember(Name =UserProfileDataMemberNames.Location)]
        public AbstractLocation Location { get { return _Location; } protected set { _Location = value; 
                _LocationSet = true; } }

        private bool _LocationVisibleToSet;
        private VisibleTo _LocationVisibleTo;
        [JsonPropertyName(UserProfileDataMemberNames.LocationVisibleTo)]
        [JsonInclude]
        [DataMember(Name = UserProfileDataMemberNames.LocationVisibleTo)]
        public VisibleTo LocationVisibleTo
        {
            get { return _LocationVisibleTo; }
            set { _LocationVisibleTo = value; _LocationVisibleToSet = true; }
        }
        [JsonPropertyName(UserProfileDataMemberNames.Pictures)]
        [JsonInclude]
        [DataMember(Name =UserProfileDataMemberNames.Pictures)]
        public UserMultimediaItem[] Pictures { get; protected set; }
        public UserMultimediaItem GetPicture(string multimediaToken) {
            return Pictures?.Where(p => p.MultimediaToken == multimediaToken).FirstOrDefault();
        }
        public void AddPicture(UserMultimediaItem picture) {
            if (Pictures == null) {
                Pictures = new UserMultimediaItem[] { picture };
                return;
            }
            UserMultimediaItem[] newPictures = new UserMultimediaItem[Pictures.Length+1];
            Array.Copy(Pictures, 0, newPictures, 1, Pictures.Length);
            newPictures[0] = picture;
            Pictures = newPictures;
        }
        public bool RemovePicture(string multimediaToken) { 
            if(Pictures == null)
                return false;
            UserMultimediaItem[] newPictures = Pictures.Where(p=>p.MultimediaToken!= multimediaToken).ToArray();
            bool deleted = newPictures.Length < Pictures.Length;
            Pictures = newPictures;
            return deleted;
        }
        public UserProfile(long userId, bool guest, string username) {
            _UserId = userId;
            Username = username;
            Guest = guest;
        }
        protected UserProfile() { }
        private void SelectSummaryPictures(
            out UserMultimediaItem mainPicture, out UserMultimediaItem mainChildFriendlyPicture)
        {

            UserMultimediaItem firstMainPicture = Pictures?.Where(p => p.SetAsMain != null).FirstOrDefault();
            if (firstMainPicture == null) {
                mainPicture = null;
                mainChildFriendlyPicture = null;
                return;
            }
            if (firstMainPicture.IsChildFriendly)
            {
                mainPicture = firstMainPicture;
                mainChildFriendlyPicture = firstMainPicture;
                return;
            }
            mainPicture = firstMainPicture;
            mainChildFriendlyPicture = Pictures.Where(p => (p.SetAsMain != null) && p.IsChildFriendly).FirstOrDefault();

        }
        public UserProfileSummary ToPublicSummary(AssociateType myAssociateTypesOnUser)
        {
            UserProfileSummary userProfileSummary = new UserProfileSummary(_UserId, myAssociateTypesOnUser);
            userProfileSummary.Username = Username;
            if (_AboutYouVisibleTo.HasFlag(VisibleTo.Public))
            {
                userProfileSummary.AboutYou = _AboutYou;
            }
            SelectSummaryPictures(out UserMultimediaItem mainPicture, out UserMultimediaItem mainChildFriendlyPicture);
            userProfileSummary.MainPicture = mainPicture?.MultimediaToken;
            userProfileSummary.MainChildFriendlyPicture = mainChildFriendlyPicture?.MultimediaToken;
            return userProfileSummary;
        }
        public UserProfileSummary ToSummary(AssociateType myAssociateTypesOnUser)
        {
            UserProfileSummary userProfileSummary = new UserProfileSummary(_UserId, myAssociateTypesOnUser);
            userProfileSummary.Username = Username;
            Func<VisibleTo, bool> allowedToSee = Get_AllowedToSee(myAssociateTypesOnUser);
            if (allowedToSee(_FirstNameVisibleTo))
                userProfileSummary.FirstName = _FirstName;
            if (allowedToSee(_MiddleNamesVisibleTo))
             userProfileSummary.MiddleNames = _MiddleNames;
            if (allowedToSee(_SurnameVisibleTo))
                userProfileSummary.Surname = _Surname;
            if (allowedToSee(_AboutYouVisibleTo))
                userProfileSummary.AboutYou = _AboutYou;
            SelectSummaryPictures(out UserMultimediaItem mainPicture, out UserMultimediaItem mainChildFriendlyPicture);
            userProfileSummary.MainPicture = mainPicture?.MultimediaToken;
            userProfileSummary.MainChildFriendlyPicture = mainChildFriendlyPicture?.MultimediaToken;
            //if (allowedToSee(_LocationVisibleTo))
            //  userProfileSummary.Location = _Location;
            return userProfileSummary;
        }
        public UserProfile ToFilteredProfile(AssociateType myAssociateTypesOnUser)
        {
            Func<VisibleTo, bool> allowedToSee = Get_AllowedToSee(myAssociateTypesOnUser);

            UserProfile userProfile = new UserProfile(_UserId, Guest, Username);
            userProfile.Username = Username;
            if (allowedToSee(_TitleVisibleTo))
            {
                userProfile._Title = _Title;
            }
            if (allowedToSee(_FirstNameVisibleTo))
            {
                userProfile._FirstName = _FirstName;
            }
            if (allowedToSee(_MiddleNamesVisibleTo))
            {
                userProfile._MiddleNames = _MiddleNames;
            }
            if (allowedToSee(_SurnameVisibleTo))
            {
                userProfile._Surname = _Surname;
            }
            if (allowedToSee(_LocationVisibleTo))
            {
                userProfile._Location = _Location;
            }
            if (allowedToSee(_AboutYouVisibleTo))
            {
                userProfile._AboutYou = _AboutYou;
            }
            if (allowedToSee(_MobilePhoneNumberVisibleTo))
            {
                userProfile._MobilePhoneNumber = _MobilePhoneNumber;
            }
            if (allowedToSee(_WorkEmailVisibleTo))
            {
                userProfile._WorkEmail = _WorkEmail;
            }
            if (allowedToSee(_WorkPhoneNumberVisibleTo))
            {
                userProfile._WorkPhoneNumber = _WorkPhoneNumber;
            }
            if (allowedToSee(_ContactEmailVisibleTo))
            {
                userProfile._ContactEmail = _ContactEmail;
            }
            if (allowedToSee(_GenderVisibleTo))
            {
                userProfile._Gender = _Gender;
            }
            if (allowedToSee(_BirthdayVisibleTo))
            {
                userProfile._Birthday = _Birthday;
            }
            return userProfile;
        }
        public void Update(UserProfile newUserProfile)
        {
            if (newUserProfile._UsernameSet)
            {
                _Username = newUserProfile._Username;
            }
            if (newUserProfile._TitleSet) {
                _Title = newUserProfile._Title;
            }
            if (newUserProfile._TitleVisibleToSet)
            {
                _TitleVisibleTo = newUserProfile._TitleVisibleTo;
            }
            if (newUserProfile._FirstNameSet)
            {
                _FirstName = newUserProfile._FirstName;
            }
            if (newUserProfile._FirstNameVisibleToSet)
            {
                _FirstNameVisibleTo = newUserProfile._FirstNameVisibleTo;
            }
            if (newUserProfile._MiddleNamesSet)
            {
                _MiddleNames = newUserProfile._MiddleNames;
            }
            if (newUserProfile._MiddleNamesVisibleToSet)
            {
                _MiddleNamesVisibleTo = newUserProfile._MiddleNamesVisibleTo;
            }
            if (newUserProfile._SurnameSet)
            {
                _Surname = newUserProfile._Surname;
            }
            if (newUserProfile._SurnameVisibleToSet)
            {
                _SurnameVisibleTo = newUserProfile._SurnameVisibleTo;
            }
            if (newUserProfile._LocationSet)
            {
                _Location = newUserProfile._Location;
            }
            if (newUserProfile._LocationVisibleToSet)
            {
                _LocationVisibleTo = newUserProfile._LocationVisibleTo;
            }
            if (newUserProfile._AboutYouSet)
            {
                _AboutYou = newUserProfile._AboutYou;
            }
            if (newUserProfile._AboutYouVisibleToSet)
            {
                _AboutYouVisibleTo = newUserProfile._AboutYouVisibleTo;
            }
            if (newUserProfile._MobilePhoneNumberSet)
            {
                _MobilePhoneNumber = newUserProfile._MobilePhoneNumber;
            }
            if (newUserProfile._MobilePhoneNumberVisibleToSet)
            {
                _MobilePhoneNumberVisibleTo = newUserProfile._MobilePhoneNumberVisibleTo;
            }
            if (newUserProfile._WorkPhoneNumberSet)
            {
                _WorkPhoneNumber = newUserProfile._WorkPhoneNumber;
            }
            if (newUserProfile._WorkPhoneNumberVisibleToSet)
            {
                _WorkPhoneNumberVisibleTo = newUserProfile._WorkPhoneNumberVisibleTo;
            }
            if (newUserProfile._ContactEmailSet)
            {
                _ContactEmail = newUserProfile._ContactEmail;
            }
            if (newUserProfile._ContactEmailVisibleToSet)
            {
                _ContactEmailVisibleTo = newUserProfile._ContactEmailVisibleTo;
            }
            if (newUserProfile._WorkEmailSet)
            {
                _WorkEmail = newUserProfile._WorkEmail;
            }
            if (newUserProfile._WorkEmailVisibleToSet)
            {
                _WorkEmailVisibleTo = newUserProfile._WorkEmailVisibleTo;
            }
            if (newUserProfile._GenderSet)
            {
                _Gender = newUserProfile._Gender;
            }
            if (newUserProfile._GenderVisibleToSet)
            {
                _GenderVisibleTo = newUserProfile._GenderVisibleTo;
            }
            if (newUserProfile._BirthdaySet)
            {
                _Birthday = newUserProfile._Birthday;
            }
            if (newUserProfile._BirthdayVisitleToSet)
            {
                _BirthdayVisibleTo = newUserProfile._BirthdayVisibleTo;
            }
        }
        public string GetFullName()
        {
            List<string> strs = new List<string>();
            strs.Add(Title.GetString());
            if (!string.IsNullOrEmpty(FirstName))
                strs.Add(FirstName);
            if (MiddleNames != null)
                foreach (string middleName in MiddleNames)
                    strs.Add(middleName);
            if (!string.IsNullOrEmpty(Surname))
                strs.Add(Surname);
            return string.Join(' ', strs);
        }
        public FrequentlyAccessedUserProfile ToFrequentlyAccessed() {
            return new FrequentlyAccessedUserProfile(GetFullName());
        }
        private Func<VisibleTo, bool> Get_AllowedToSee(AssociateType myAssociateTypesOnUser) {
            return (visibleTo) => {
                if ((visibleTo & VisibleTo.MeOnly) > 0)
                    return false;
                return
                /*Public*/((visibleTo & VisibleTo.Public) > 0)||
                /*Shared with my associate type*/((((VisibleTo)myAssociateTypesOnUser) & visibleTo) > 0);
            };
        }

    }
}
