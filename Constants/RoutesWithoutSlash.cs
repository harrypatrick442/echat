namespace GlobalConstants
{
    public static class RoutesWithoutSlash
    {
        public const string THROUGH_SERVER_SEND= "send",
            THROUGH_SERVER_RECEIVE="receive",
            THROUGH_SERVER_PROGRESS = "progress",
            THROUGH_SERVER_PING = "ping",
            THROUGH_SERVER_SENDER_CANCEL = "senderCancel",
            THROUGH_SERVER_RECEIVER_CANCEL = "receiverCancel";


        [ExportToJavaScript]
        public const string LOG_SERVER_LOG_ERROR = "logError";
        [ExportToJavaScript]
        public const string LOG_SERVER_LOG_SESSION = "logSession";
        [ExportToJavaScript]
        public const string LOG_SERVER_LOG_BREADCRUMB = "logBreadcrumb";

        [ExportToJavaScript]
        public const string LOG_VIEWER_AUTHENTICATE_WITH_TOKEN = "logViewer/authenticateWithToken";
        [ExportToJavaScript]
        public const string LOG_VIEWER_SIGN_IN = "logViewer/signIn";
        [ExportToJavaScript]
        public const string LOG_VIEWER_GET_ERROR = "logViewer/getError";
        [ExportToJavaScript]
        public const string LOG_VIEWER_GET_ERROR_IDS = "logViewer/getErrorIds";
        [ExportToJavaScript]
        public const string LOG_VIEWER_GET_SESSION = "logViewer/getSession";
        [ExportToJavaScript]
        public const string LOG_VIEWER_GET_SESSION_IDS = "logViewer/getSessionIds";
        [ExportToJavaScript]
        public const string LOG_VIEWER_GET_BREADCRUMB = "logViewer/getBreadcrumb";
        [ExportToJavaScript]
        public const string LOG_VIEWER_GET_BREADCRUMB_IDS = "logViewer/getBreadcrumbIds";

        [ExportToJavaScript]
        public const string MAINTENANCE_CLIENT_CONTROLLER_AUTHENTICATE_WITH_TOKEN = "authenticateWithToken";
        [ExportToJavaScript]
        public const string MAINTENANCE_CLIENT_CONTROLLER_SIGN_IN = "signIn";
        [ExportToJavaScript]
        public const string MAINTENANCE_CLIENT_CONTROLLER_NEW_SCHEDULED_MAINTINANCE = "newScheduledMaintenance";
        [ExportToJavaScript]
        public const string MULTIMEDIA_SERVER_UPLOAD = "u";
        [ExportToJavaScript]
        public const string MULTIMEDIA_SERVER_MULTIMEDIA = "m";

        public const string ID_SERVER_GET = "idg";
        public const string RESET_PASSWORD_CLICKED_LINK = "rp";
        public const string RESET_PASSWORD_BY_EMAIL = "rpbe";
        public const string RESET_PASSWORD_UPDATE_PASSWORD = "rpup";
    }
}