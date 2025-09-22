using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Core.Timing;
using Core.LoadBalancing;
using Statistics;
using WebAbstract;
using FileServerCore;
using DependencyManagement;
using Logging;

namespace FileServerBase
{
    [EnableCors("FileServerController")]
    public class FileServerController : ControllerBase
    {
        private static readonly object _LockObjectFilesRelayIndex = new object(),
            _LockObjectEChatIndex = new object();
        private static FilesRelayIndex _FilesRelayIndex;
        private static EChatIndex _EChatIndex;

        private static readonly string[] _FilesRelayHosts = new string[] {
            "filesrelay.com" };
        private static readonly string[] _EChatHosts = new string[] {
            "dev.e-chat.live", "e-chat.live" };
        //This stays under the above. Order is important here. 
        private static FileServer.DynamicFileServer _FileServer = 
            new FileServer.DynamicFileServer(
                DependencyManager.GetString(FileServerCore.DependencyNames.ClientDirectoryPath),
                ProvideDynamicCachedFile);
        public FileServerController() : base()
        {

        }
        [HttpGet]
        [Route("{*any}")]
        public ActionResult Any()
        {
            string path = Request.Path.Value;
            Logs.Default.Info("GOT REQUEST");
            Logs.Default.Info(path);
            if (path.IndexOf("/d/") == 0)
            { 
                string[] splits = path.Split('/');
                if (splits[2].Length==32)
                    path = "/";
                else
                {
                    path = path.Substring(2, path.Length - 2);
                }
            }
            Logs.Default.Info(path);
            DynamicCachedFilesHost cachedFilesForHost= _FileServer.GetCachedFilesForHost(Request.Host.Host);
            if (cachedFilesForHost == null) 
                return StatusCode(404);
            Logs.Default.Info("a");
            IDynamicCachedFile dynamicCachedFile = cachedFilesForHost.GetCachedFile(path);
            if (dynamicCachedFile==null)
            {
                return StatusCode(404);
            }
            Logs.Default.Info("b");
            byte[] bytes = dynamicCachedFile.GetBytes(out string contentType);
            if (bytes == null) 
                return StatusCode(404);
            Logs.Default.Info("c");
            if (dynamicCachedFile.IsIndex)
            {
                Logs.Default.Info("d");
                LogIndexFile();
            }
            return new FileContentResult(bytes, contentType);
        }
        private void LogIndexFile()
        {

            Logs.Default.Info("LogIndexFile");
            string userAgent = Request.Headers["User-Agent"];
            FileServerStatisticsFileLogger.Instance.Log(
                new FileServerEntry(TimeHelper.MillisecondsNow,
                    IpHelper.GetClientIPAddress(Request),
                    userAgent, StatisticsEntryType.StaticFileServerLoadIndex));
        }
        private static IDynamicCachedFile ProvideDynamicCachedFile(string filePath, string relativePath,
            bool isIndex, DynamicCachedFilesHost host)
        {
            Logs.Default.Info("ProvideDynamicCachedFile");
            if (!isIndex)
                return null;
            if (_FilesRelayHosts.Contains(host.Host))
                return FilesRelayIndexDynamicCachedFile(filePath, relativePath, isIndex, host);
            if (_EChatHosts.Contains(host.Host))
                return EChatIndexDynamicCachedFile(filePath, relativePath, isIndex, host);
            return null;
            //throw new NotImplementedException($"Not implemented for host \"{host.Host}\"");
        }
        private static IDynamicCachedFile FilesRelayIndexDynamicCachedFile(string filePath, string relativePath,
            bool isIndex, DynamicCachedFilesHost host)
        {
            Logs.Default.Info("FilesRelayIndexDynamicCachedFile");
            FilesRelayIndex cachedFile;
            lock (_LockObjectFilesRelayIndex)
            {
                cachedFile = _FilesRelayIndex;
            }
            if (cachedFile != null)
            {
                ReceivingLoadBalancer.Instance.RemoveHandler(cachedFile);
            }
            cachedFile = new FilesRelayIndex(
                filePath, relativePath, GlobalConstants.Nodes.FILES_RELAY_1);
            lock (_LockObjectFilesRelayIndex)
            {
                _FilesRelayIndex = cachedFile;
            }
            ReceivingLoadBalancer.Instance.AddHandler(cachedFile);
            return cachedFile;
        }
        private static IDynamicCachedFile EChatIndexDynamicCachedFile(string filePath, string relativePath,
            bool isIndex, DynamicCachedFilesHost host)
        {
            Logs.Default.Info("EChatIndexDynamicCachedFile");
            EChatIndex cachedFile;
            lock (_LockObjectEChatIndex)
            {
                cachedFile = _EChatIndex;
            }
            if (cachedFile != null)
            {
                ReceivingLoadBalancer.Instance.RemoveHandler(cachedFile);
            }
            cachedFile = new EChatIndex(
                filePath, relativePath, GlobalConstants.Nodes.ECHAT_1);
            lock (_LockObjectEChatIndex)
            {
                _EChatIndex = cachedFile;
            }
            ReceivingLoadBalancer.Instance.AddHandler(cachedFile);
            return cachedFile;
        }
    }
}