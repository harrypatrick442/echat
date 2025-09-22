using Core.Handlers;
using Users.Messages.Client;
using InterserverComs;
using MessageTypes.Internal;
using Core;

namespace Users.FrequentlyAccessedUserProfiles
{
    public partial class FrequentlyAccessedUserProfilesManager
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        protected void Initialize_Server()
        {
            _MessageTypeMappingsHandler = InterserverMessageTypeMappingsHandler.Instance;
            _MessageTypeMappingsHandler.AddRange(
                new TupleList<string, DelegateHandleMessageOfType<InterserverMessageEventArgs>> {
                    {InterserverMessageTypes.UserFrequentlyAccessedUserProfileUpdate, HandleFrequentlyAccessedUserProfileUpdate }
                }
            );
        }
        private void HandleFrequentlyAccessedUserProfileUpdate(InterserverMessageEventArgs e)
        {
            FrequentlyAccessedUserProfileUpdateRequest request = e.Deserialize<FrequentlyAccessedUserProfileUpdateRequest>();
            Set_Here(request.UserId, request.FrequentlyAccessedUserProfile);
        }
    }
}