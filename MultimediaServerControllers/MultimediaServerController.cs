using Microsoft.AspNetCore.Mvc;
using Logging;
using Microsoft.AspNetCore.Cors;
using Statistics;
using WebAbstract;
using Core.Timing;
using MultimediaServerCore;
using Core.FileSystem;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace MultimediaServer
{

    //[FeatureGate(Features.ThroughServer)]
    [EnableCors("MultimediaServerCors")]
    public class MultimediaServerController : ControllerBase
    {
        public MultimediaServerController() : base()
        {

        }
        [HttpPost]
        [Route(GlobalConstants.Routes.MULTIMEDIA_SERVER_UPLOAD)]
        [RequestSizeLimit(GlobalConstants.Sizes.MULTIMEDIA_SERVER_MAXIMUM_FILE_SIZE)]
        public IActionResult Upload()
        {
            Logs.Default.Info("Got the request for multimedia to upload");
            string token = _GetToken();
            Logs.Default.Info(token);
            StatisticsLog(StatisticsEntryType.MultimediaServerUpload);
            try
            {
                Logs.Default.Info("in try");
                PendingMultimediaUpload? pendingMultimediaUpload = PendingMultimediaUploads.Instance.Take(token);
                if (pendingMultimediaUpload == null)
                    return StatusCode(500);
                Logs.Default.Info("no 500");
                try 
                {
                    UploadedMultimediaProcessor.ProcessMultimedia(Request.Body, pendingMultimediaUpload);
                }
                catch(Exception ex){
                    Logs.Default.Error(ex);
                }
                Logs.Default.Info("returning ok");
                return Ok();
            }
            catch (Exception ex) {
                Logs.Default.Error(ex);
                return StatusCode(500);
            }
        }
        [HttpGet]
        [Route(GlobalConstants.Routes.MULTIMEDIA_SERVER_MULTIMEDIA + "{*any}")]
        public IActionResult Multimedia()
        {
            string? path = Request.Path.Value;
            if(path==null)
                return StatusCode(400);
            path = path.Substring(3, path.Length - 3);
            byte[]? bytes = MultimediaCache.Instance.Get(path, out string contentType);
            if (bytes == null)
                return StatusCode(404);
            string? rangeString = Request.Headers["Range"];
            if (string.IsNullOrEmpty(rangeString))
                return new FileContentResult(bytes, contentType);
            var range = rangeString.Split('=', '-');
            var startByteIndex = int.Parse(range[1]);
            if (!long.TryParse(range[2], out long endByteIndex))
            {
                endByteIndex = bytes.Length - 1;
            }
            if (endByteIndex >= bytes.Length)
            {
                return StatusCode(406);
            }
            int contentLength = (int)(endByteIndex - startByteIndex + 1);
            Response.StatusCode = 206;
            Response.Headers.Add("Content-Range", $"bytes {startByteIndex}-{endByteIndex}/{bytes.Length}");
            Response.Headers.Add("Content-Length", contentLength.ToString());
            var stream = new MemoryStream(bytes, startByteIndex, contentLength, false);
            return new FileStreamResult(stream, contentType);
        }
        private string _GetToken()
        {
            string? token = Request.Query[GlobalConstants.Parameters.MULTIMEDIA_TOKEN];
            if (string.IsNullOrEmpty(token)) 
                throw new ArgumentException($"token \"{GlobalConstants.Parameters.MULTIMEDIA_TOKEN}=...\"");
            return token;
        }
        private void StatisticsLog(StatisticsEntryType entryType) {
            string ip = IpHelper.GetClientIPAddress(Request);
            MultimediaServerStatisticsFileLogger.Instance.Log(
                new MultimediaServerEntry(TimeHelper.MillisecondsNow, ip, null, entryType)
            );
        }
    }
}