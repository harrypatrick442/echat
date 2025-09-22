using Core.Ticketing;

namespace KeyValuePairDatabases
{
    public static class RemoteOperationResponseDataMemberNames
    {
        public const string Payload = "p";
        public const string Success = "s";
        public const string ErrorMessage ="e";
        public const string InverseTicket = InverseTicketedDataMemberNames.InverseTicket;
    }
}