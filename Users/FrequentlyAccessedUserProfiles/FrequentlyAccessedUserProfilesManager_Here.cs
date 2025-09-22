using Users.DAL;

namespace Users.FrequentlyAccessedUserProfiles
{
    public partial class FrequentlyAccessedUserProfilesManager
    {
        public void Set_Here(long userId, FrequentlyAccessedUserProfile frequentlyAccessedUserProfile)
        {
            DalFrequentlyAccessedUserProfiles.Instance.SetHere(userId, frequentlyAccessedUserProfile);
        }
    }
}