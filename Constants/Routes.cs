namespace GlobalConstants
{
    public static class Routes
    {
        public const string RESET_PASSWORD_CLICKED_LINK = RoutesWithoutSlash.RESET_PASSWORD_CLICKED_LINK + "/{*any}";
        public const string RESET_PASSWORD_BY_EMAIL = RoutesWithoutSlash.RESET_PASSWORD_BY_EMAIL + "/{*any}";
        public const string RESET_PASSWORD_UPDATE_PASSWORD = RoutesWithoutSlash.RESET_PASSWORD_UPDATE_PASSWORD + "/";
        public const string ID_SERVER_GET = RoutesWithoutSlash.ID_SERVER_GET + "/";
        public const string LOG_SERVER_LOG_ERROR = RoutesWithoutSlash.LOG_SERVER_LOG_ERROR + "/";
        public const string LOG_SERVER_LOG_SESSION = RoutesWithoutSlash.LOG_SERVER_LOG_SESSION + "/";
        public const string LOG_SERVER_LOG_BREADCRUMB = RoutesWithoutSlash.LOG_SERVER_LOG_BREADCRUMB + "/";

        public const string LOG_VIEWER_AUTHENTICATE_WITH_TOKEN = RoutesWithoutSlash.LOG_VIEWER_AUTHENTICATE_WITH_TOKEN + "/";
        public const string LOG_VIEWER_SIGN_IN = RoutesWithoutSlash.LOG_VIEWER_SIGN_IN + "/";
        public const string LOG_VIEWER_GET_ERROR = RoutesWithoutSlash.LOG_VIEWER_GET_ERROR + "/";
        public const string LOG_VIEWER_GET_ERROR_IDS = RoutesWithoutSlash.LOG_VIEWER_GET_ERROR_IDS + "/";
        public const string LOG_VIEWER_GET_SESSION = RoutesWithoutSlash.LOG_VIEWER_GET_SESSION + "/";
        public const string LOG_VIEWER_GET_SESSION_IDS = RoutesWithoutSlash.LOG_VIEWER_GET_SESSION_IDS + "/";
        public const string LOG_VIEWER_GET_BREADCRUMB = RoutesWithoutSlash.LOG_VIEWER_GET_BREADCRUMB + "/";
        public const string LOG_VIEWER_GET_BREADCRUMB_IDS = RoutesWithoutSlash.LOG_VIEWER_GET_BREADCRUMB_IDS + "/";

        public const string MAINTENANCE_CLIENT_CONTROLLER_AUTHENTICATE_WITH_TOKEN = RoutesWithoutSlash.MAINTENANCE_CLIENT_CONTROLLER_AUTHENTICATE_WITH_TOKEN + "/";
        public const string MAINTENANCE_CLIENT_CONTROLLER_SIGN_IN = RoutesWithoutSlash.MAINTENANCE_CLIENT_CONTROLLER_SIGN_IN + "/";
        public const string MAINTENANCE_CLIENT_CONTROLLER_NEW_SCHEDULED_MAINTINANCE = RoutesWithoutSlash.MAINTENANCE_CLIENT_CONTROLLER_NEW_SCHEDULED_MAINTINANCE + "/";

        public const string TRANSFER_SERVER_SEND = RoutesWithoutSlash.THROUGH_SERVER_SEND + "/";
        public const string TRANSFER_SERVER_RECEIVE = RoutesWithoutSlash.THROUGH_SERVER_RECEIVE + "/";
        public const string TRANSFER_SERVER_PROGRESS = RoutesWithoutSlash.THROUGH_SERVER_PROGRESS + "/";
        public const string TRANSFER_SERVER_PING = RoutesWithoutSlash.THROUGH_SERVER_PING + "/";
        public const string TRANSFER_SERVER_SENDER_CANCEL = RoutesWithoutSlash.THROUGH_SERVER_SENDER_CANCEL + "/";
        public const string TRANSFER_SERVER_RECEIVER_CANCEL = RoutesWithoutSlash.THROUGH_SERVER_RECEIVER_CANCEL + "/";
        [ExportToJavaScript]
        public const string MULTIMEDIA_SERVER_UPLOAD = "u/";
        [ExportToJavaScript]
        public const string MULTIMEDIA_SERVER_MULTIMEDIA = "m/";
    }
}