
using JSON;
using Logging;
using InterserverComs;

namespace KeyValuePairDatabases.Appended
{
    public partial class AppendedKeyValuePairOnDiskDatabase<TEntry>
    {
        public void HandleAppendedRead(InterserverMessageEventArgs e, AppendedReadRequest request)
        {
            AppendedReadResponse response;
            try
            {
                string[] entries = _Read_Here(request.Identifier,
                    request.IndexToReadFromBackwardsExclusive, request.NEntries,
                    out long toIndexFromBeginningExclusive).ToArray();
                 response = AppendedReadResponse.Success(entries.ToArray(),
                    toIndexFromBeginningExclusive, request.Ticket);
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                response = AppendedReadResponse.Failed(request.Ticket);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        public void HandleAppendedAppend(InterserverMessageEventArgs e, AppendedAppendRequest request)
        {
            AppendedAppendResponse response;
            try
            {
                _Append_Here(request.Identifier, request.Entry, out long indexToContinueFromToGoBackFromMessage);
                response = new AppendedAppendResponse(true, indexToContinueFromToGoBackFromMessage, request.Ticket);
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
                response = new AppendedAppendResponse(false, null, request.Ticket);
            }
            try
            {
                e.EndpointFrom.SendJSONString(Json.Serialize(response));
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
    }
}