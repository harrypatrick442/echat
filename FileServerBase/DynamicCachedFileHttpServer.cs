using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;
using Core.Exceptions;
using Logging;
using SnippetsCore;
using Core.Assets;
using FileServerBase;

namespace Snippets
{
    public class DynamicCachedFileHttpServer : IDisposable
    {
        private FileServer.DynamicFileServer _FileServer = new FileServer.DynamicFileServer(Paths.ClientDirectoryPath);

        private object _LockObjectDispose = new object();
        private CancellationTokenSource _CancellationTokenSourceDisposed = new CancellationTokenSource();
        private bool _AllowCors;
        public DynamicCachedFileHttpServer()
        {
            _AllowCors = true;
            CountdownLatch countdownLatchListeningOrFailed = new CountdownLatch();
            new Thread(() => Listen(countdownLatchListeningOrFailed)).Start();
            countdownLatchListeningOrFailed.Wait();
        }
        ~DynamicCachedFileHttpServer()
        {
            Dispose();
        }
        public void Dispose()
        {
            lock (_LockObjectDispose)
            {
                if (_CancellationTokenSourceDisposed.IsCancellationRequested) return;
                _CancellationTokenSourceDisposed.Cancel();
            }
        }

        private void Listen(CountdownLatch countdownLatchListeningOrFailed)
        {
            try
            {
                using (HttpListener httpListener = new HttpListener())
                {
                    using (CancellationTokenRegistration cancellationTokenRegistration =
                        _CancellationTokenSourceDisposed.Token.Register(httpListener.Abort))
                    {
                        httpListener.Prefixes.Add($"http://*:80/");
                        httpListener.Start();
                        countdownLatchListeningOrFailed.Signal();
                        while (!_CancellationTokenSourceDisposed.IsCancellationRequested)
                        {
                            try
                            {
                                IAsyncResult result = httpListener.BeginGetContext(ListenerCallback, httpListener);
                                result.AsyncWaitHandle.WaitOne();
                            }
                            catch (Exception ex)
                            {
                                Logs.Default.Error(ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                countdownLatchListeningOrFailed.Signal();
                Logs.Default.Error(ex);
            }
        }
        public void ListenerCallback(IAsyncResult result)
        {
            if (_CancellationTokenSourceDisposed.IsCancellationRequested)
            {
                return;
            }
            try
            {   
                HttpListener listener = (HttpListener)result.AsyncState;
                HttpListenerContext context = listener.EndGetContext(result);
                Process(context);
            }
            catch (ObjectDisposedException ex) {
                Logs.Default.Error(ex);
            }
        }
        private void Process(HttpListenerContext httpListenerContext)
        {
            try
            {
                var request = httpListenerContext.Request;
                DynamicCachedFilesHost dynamicCachedFilesHost = _FileServer.GetCachedFilesForHost(request.UserHostName.Split(":")[0]);
                if (dynamicCachedFilesHost == null)
                {
                    httpListenerContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }
                byte[] bytes = dynamicCachedFilesHost.GetBytes(request.Url.LocalPath, out string contentType);
                if (bytes == null)
                {
                    httpListenerContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }
                ReturnFile(bytes, contentType, httpListenerContext.Response);
            }
            catch (Exception ex)
            {
                httpListenerContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            finally
            {
                httpListenerContext.Response.OutputStream.Close();
            }
        }
        private void ReturnFile(byte[] bytes, string contentType, HttpListenerResponse httpListenerResponse)
        {
                httpListenerResponse.ContentType = contentType;
                httpListenerResponse.ContentLength64 = bytes.Length;
                httpListenerResponse.AddHeader("Date", DateTime.Now.ToString("r"));
                //httpListenerResponse.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filePath).ToString("r"));
                if (_AllowCors)
                    AddCorsHeaders(httpListenerResponse);
                httpListenerResponse.OutputStream.Write(bytes);
                httpListenerResponse.StatusCode = (int)HttpStatusCode.OK;
                httpListenerResponse.OutputStream.Flush();
        }
        private void AddCorsHeaders(HttpListenerResponse httpListenerResponse)
        {
            httpListenerResponse.AddHeader("Access-Control-Allow-Origin", "*");
        }
    }
}