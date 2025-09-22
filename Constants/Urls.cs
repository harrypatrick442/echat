namespace GlobalConstants
{
    public static class Urls
    {
        public const string LOG_SERVER_ROOT ="https://log.objmesh.com/";
        public const string LOG_SERVER_LOG_ERROR = Urls.LOG_SERVER_ROOT+RoutesWithoutSlash.LOG_SERVER_LOG_ERROR;
        public const string LOG_SERVER_LOG_SESSION = Urls.LOG_SERVER_ROOT + RoutesWithoutSlash.LOG_SERVER_LOG_SESSION;
        public const string LOG_SERVER_LOG_BREADCRUMB = Urls.LOG_SERVER_ROOT + RoutesWithoutSlash.LOG_SERVER_LOG_BREADCRUMB;
        public const string LOG_VIEWER_AUTHENTICATE_WITH_TOKEN = Urls.LOG_SERVER_ROOT + RoutesWithoutSlash.LOG_VIEWER_AUTHENTICATE_WITH_TOKEN;
        public const string LOG_VIEWER_GET_BREADCRUMB = Urls.LOG_SERVER_ROOT + RoutesWithoutSlash.LOG_VIEWER_GET_BREADCRUMB;
        public const string LOG_VIEWER_GET_BREADCRUMB_IDS = Urls.LOG_SERVER_ROOT + RoutesWithoutSlash.LOG_VIEWER_GET_BREADCRUMB_IDS;
        public const string LOG_VIEWER_GET_ERROR = Urls.LOG_SERVER_ROOT + RoutesWithoutSlash.LOG_VIEWER_GET_ERROR;
        public const string LOG_VIEWER_GET_ERROR_IDS = Urls.LOG_SERVER_ROOT + RoutesWithoutSlash.LOG_VIEWER_GET_ERROR_IDS;
        public const string LOG_VIEWER_GET_SESSION = Urls.LOG_SERVER_ROOT + RoutesWithoutSlash.LOG_VIEWER_GET_SESSION;
        public const string LOG_VIEWER_GET_SESSION_IDS = Urls.LOG_SERVER_ROOT + RoutesWithoutSlash.LOG_VIEWER_GET_SESSION_IDS;
        public const string LOG_VIEWER_SIGN_IN = Urls.LOG_SERVER_ROOT + RoutesWithoutSlash.LOG_VIEWER_SIGN_IN;
    }
}