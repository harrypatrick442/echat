using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using EnvironmentVariables;
using FileServerCore;

namespace LogServer.Controllers
{
    //[FeatureGate(Features.LogServerClient)]
    [EnableCors("LogServerCors")]
    public class LogServerIndexController : ControllerBase
    {
        private static readonly CachedIndexFile _CachedIndexFile = new CachedIndexFile(Paths.Client_LogViewer);
        private static readonly CachedIndexFile _CachedIndexFileMaintenance = new CachedIndexFile(Paths.Client_MaintenanceClient);
        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            if (Request.Scheme == "http")
            {
                string queryString = !Request.QueryString.HasValue
                    ? "" : Request.QueryString.Value;
                string hostname = Request.Host.ToString();
                if (Request.Host.Port == 7161)
                {
                    hostname = Request.Host.Host;
                }
                return new RedirectResult($"https://{hostname}{Request.Path}{queryString}");
            }
            return new ContentResult
            {
                ContentType = "text/html",
                Content = Request.Host.Host.Contains("maintenance")? _CachedIndexFileMaintenance.Content:_CachedIndexFile.Content
            };
        }

    }
}