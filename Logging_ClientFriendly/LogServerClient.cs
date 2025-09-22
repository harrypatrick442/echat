
using Core.Enums;
using Shutdown;
using Core;
using Ajax;
using Core.Exceptions;
using JSON;
using Core.Threading;
using Core.Timing;
using System;
using System.Threading;
using Logging;
using Logging_ClientFriendly.Messages;

namespace Logging_ClientFriendly
{
    public class LogServerClient : ILogServerClient
    {
        private static LogServerClient _Instance;
        public static LogServerClient Initialize(Platform platform, Project project, long? nodeId)
        {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(LogServerClient));
            _Instance = new LogServerClient(platform, project, nodeId);
            return _Instance;
        }
        public static LogServerClient Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(LogServerClient));
                return _Instance;
            }
        }
        private Platform _Platform;
        private Project _Project;
        private long? _NodeId;
        private long _SessionId;
        public long SessionId { get { return _SessionId; } }
        private LogServerClient(Platform platform, Project project, long? nodeId)
        {
            _Platform = platform;
            _Project = project;
            _NodeId = nodeId;
            _Session();
        }
        public void Error(Exception ex)
        {
            try
            {
                AjaxHelper.PostWithoutWaitingForResponse(GlobalConstants.Urls.LOG_SERVER_LOG_ERROR,
                    new LoggedError(_SessionId, TimeHelper.MillisecondsNow, ex.StackTrace, ex.Message,
                    _Platform, null, nodeId: _NodeId),
                    Json.Instance, timeoutMilliseconds: 3000);
            }
            catch (Exception e) { Logs.LogggingLogger.Error(e); }
        }
        public void Error(string message)
        {
            try
            {
                AjaxHelper.PostWithoutWaitingForResponse(GlobalConstants.Urls.LOG_SERVER_LOG_ERROR,
                        new LoggedError(_SessionId, TimeHelper.MillisecondsNow, null, message, _Platform,
                        null, nodeId: _NodeId),
                    Json.Instance, timeoutMilliseconds: 3000);
            }
            catch (Exception e) { Logs.LogggingLogger.Error(e); }
        }
        public void Breadcrumb(BreadcrumbType type, string description, string value)
        {
            try
            {
                AjaxHelper.PostWithoutWaitingForResponse(GlobalConstants.Urls.LOG_SERVER_LOG_BREADCRUMB,
                        new Breadcrumb(_SessionId, TimeHelper.MillisecondsNow,
                        type, description, value),
                    Json.Instance, timeoutMilliseconds: 3000);
            }
            catch (Exception e) { Logs.LogggingLogger.Error(e); }
        }
        private void _Session()
        {
            CountdownLatch countdownLatchFirstTime = new CountdownLatch();
            Func<bool> sleep = SafeSleep.GetSleep(
                ShutdownManager.Instance.CancellationToken,
                new IntervalNTimes(2000, 3),
                new IntervalNTimes(10000, 12),
                new IntervalNTimes(120000, -1)
            );
            new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        LoggedSession loggedSession = new LoggedSession(TimeHelper.MillisecondsNow, _Platform, null,
                            _Project, null, _NodeId);
                        AjaxResult ajaxResult = AjaxHelper.PostSync(GlobalConstants.Urls.LOG_SERVER_LOG_SESSION,
                            loggedSession,
                            Json.Instance, timeoutMilliseconds: 2000);
                        if (ajaxResult.Successful)
                        {
                            _SessionId = long.Parse(ajaxResult.GetRawPayload());
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        Logs.LogggingLogger.Error(e);
                    }
                    finally
                    {
                        countdownLatchFirstTime.Signal();
                    }
                    if (sleep())
                        return;
                }
            }).Start();
            countdownLatchFirstTime.Wait();
        }
    }
}
