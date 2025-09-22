using InterserverComs;
using JSON;
using Logging;
using MentionsCore.Messages;
using MentionsCore.Requests;
using MentionsCore.Responses;

namespace MentionsCore
{
    public sealed partial class MentionsMesh
    {
        private InterserverMessageTypeMappingsHandler _MessageTypeMappingsHandler;
        private void HandleGet(InterserverMessageEventArgs e)
        {
            GetMentionsRequest request = e.Deserialize<GetMentionsRequest>();
            GetMentionsResponse response;
            try
            {
                Mention[] mentions = Get_Here(request.UserId, request.NEntries, request.IdToExclusive, request.IdFromInclusive);
                response = GetMentionsResponse.Success(mentions, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = GetMentionsResponse.Failed(request.Ticket);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
            }
        }
        private void HandleSetSeen(InterserverMessageEventArgs e)
        {
            SetSeenMention request = e.Deserialize<SetSeenMention>();
            try
            {
                SetSeen_Here(request.UserIdBeingMentioned, request.MessageId);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleAdd(InterserverMessageEventArgs e)
        {
            AddOrUpdateMention request = e.Deserialize<AddOrUpdateMention>();
            try
            {
                Add_Here(request.UserIdsBeingMentioned, request.Mention, request.IsUpdate);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}