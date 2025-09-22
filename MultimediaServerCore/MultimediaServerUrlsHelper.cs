namespace MultimediaServerCore.Messages
{
    public static class MultimediaServerUrlsHelper
    {
        private static string _UrlPrefix;
        static MultimediaServerUrlsHelper(){
            string[] domains = GlobalConstants.Nodes.UniqueDomainsForNode(Nodes.Nodes.Instance.MyId);
            string? firstMsDomain = domains.Where(d=>d.IndexOf("ms.")==0).FirstOrDefault();
            string domain = firstMsDomain!=null?firstMsDomain!:domains[0];
            _UrlPrefix = $"https://{domain}/{GlobalConstants.Routes.MULTIMEDIA_SERVER_UPLOAD}?{GlobalConstants.Parameters.MULTIMEDIA_TOKEN}=";
        }
        public static string GetUploadUrl(string token) { 
            return _UrlPrefix + token;
        }
    }
}