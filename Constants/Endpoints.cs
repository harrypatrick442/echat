namespace GlobalConstants
{
    public static class Endpoints
    {
        [ExportToJavaScript]
        public const string FILES_RELAY_WEBSOCKET = "/frws";
        [ExportToJavaScript]
        public const string ECHAT_ROOM_WEBSOCKET = "/ecr";
        [ExportToJavaScript]
        public const string ECHAT_USER_WEBSOCKET = "/ecu";
        public const string INTERSERVER_WEBSOCKET = "/its";
        [ExportToJavaScript]
        public const string MAINTENANCE_CLIENT_WEBSOCKET = "/mcws";
        public const string USER_ROUTING_TABLE_WEBSOCKET_ENDPOINT = "/urt";
        public const string ORGANISER_WEBSOCKET_ENDPOINT = "/ows";
    }
}