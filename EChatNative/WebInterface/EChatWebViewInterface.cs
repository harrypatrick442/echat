using Core.Messages.Messages;
using DataMemberNames.Client;
using Logging;
using JSON;
using Core.Exceptions;
using FilesRelayNative.Messages;
using Core.Events;
using Native.WebViewInterface;
using Core.Enums;
using Core.VirtualSockets;
using Native.Permissions;
using Native.Requests;
using Native.Responses;
using Native.Interfaces;
using Native.Messages;
using EChatNative.Messages;

namespace EChatNative
{
    public sealed class EChatWebViewInterface
    {
        private static EChatWebViewInterface? _Instance;
        public static EChatWebViewInterface Instance { 
            get {
                if (_Instance == null) 
                    throw new NotInitializedException(nameof(EChatWebViewInterface));
                return _Instance; 
            } 
        }
        public static EChatWebViewInterface Initialize(
            WebViewMessagingInterface webViewMessagingInterface,
            Action<string> openDirectoryInExternalApp,
            Action finishedLoading, Action reloadPage,
            IPermissionsHandler permissionsHandler, IStorage storage) {
            if (_Instance != null) throw new AlreadyInitializedException(nameof(EChatWebViewInterface));
            _Instance = new EChatWebViewInterface(webViewMessagingInterface,
                openDirectoryInExternalApp, finishedLoading, reloadPage,
                permissionsHandler, storage);
            return _Instance;
        }
        private WebViewMessagingInterface _WebViewMessagingInterface;
        private Action<string> _OpenDirectoryInExternalApp;
        private Action _FinishedLoading;
        private Action _ReloadPage;
        private IPermissionsHandler _PermissionsHandler;
        private IStorage _Storage;
        private EChatWebViewInterface(
            WebViewMessagingInterface webViewMessagingInterface,
            Action<string> openDirectoryInExternalApp,
            Action finishedLoading, Action reloadPage,
            IPermissionsHandler permissionsHandler, IStorage storage) {
            _WebViewMessagingInterface = webViewMessagingInterface;
            _OpenDirectoryInExternalApp = openDirectoryInExternalApp;
            _FinishedLoading = finishedLoading;
            _PermissionsHandler = permissionsHandler;
            _ReloadPage = reloadPage;
            _Storage = storage;
            _WebViewMessagingInterface.OnMessage += _HandleMessage;
        }
        public void SendPermissionsUpdate(bool hasAllRequired)
        {
            try
            {
                _WebViewMessagingInterface.MessageJavaScript(
                    new NativePermissionsUpdateMessage(hasAllRequired));
            }
            catch { }
        }
        public void SendNativePlatform()
        {
            _WebViewMessagingInterface.MessageJavaScript(
                new NativePlatformMessage(Platform.Android));
        }
        /*
        public void SendGotNewToken(string token) {
            try
            {
                _WebViewMessagingInterface.MessageJavaScript(
                    new NativeGotNewTokenMessage(token));
            }
            catch { }
        }*/
        private void _HandleMessage(object? sender, MessageEventArgs e) {
            try
            {
                TypedMessageBase typedMessage = Json.Deserialize<TypedMessageBase>(e.Message);

                Action<VirtualSocketMessage> send = _WebViewMessagingInterface.MessageJavaScript;
                switch (typedMessage.Type)
                {
                    case ClientMessageTypes.NativeReadyMessage:
                        _FinishedLoading();
                        SendNativePlatform();
                        break;
                    /*case ClientMessageTypes.NativeDownloadFile:
                        HandleDownloadFile(Deserialize<NativeDownloadFileRequest>(e));
                        break;*/
                    case ClientMessageTypes.NativeOpenDirectory:
                        NativeOpenDirectory(Deserialize<NativeOpenDirectoryRequest>(e));
                        break;
                    case ClientMessageTypes.NativeRequestPermissions:
                        _PermissionsHandler.Handle((hasAllRequired, ignore)=>SendPermissionsUpdate(hasAllRequired));
                        break;
                    case ClientMessageTypes.NativeReloadPage:
                        _ReloadPage();
                        break;
                    case ClientMessageTypes.NativeStorageGetString:
                        HandleStorageGetString(Deserialize<NativeStorageGetStringRequest>(e));
                        break;
                    case ClientMessageTypes.NativeStorageSetString:
                        HandleStorageSetString(Deserialize<NativeStorageSetStringRequest>(e));
                        break;
                    case ClientMessageTypes.NativeStorageDeleteAll:
                        HandleStorageDeleteAll(Deserialize<NativeStorageDeleteAllRequest>(e));
                        break;
                }
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
        }
        private void HandleStorageGetString(NativeStorageGetStringRequest request)
        {
            new Thread(() =>
            {
                try
                {
                    string value = _Storage.GetString(request.Key);
                    _WebViewMessagingInterface.MessageJavaScript(new NativeStorageGetStringResponse(value, request.Ticket));
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }).Start();
        }
        private void HandleStorageSetString(NativeStorageSetStringRequest request)
        {
            new Thread(() =>
            {
                try
                {
                    _Storage.SetString(request.Key, request.Value);
                    _WebViewMessagingInterface.MessageJavaScript(new NativeStorageSetStringResponse(request.Ticket));
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }).Start();
        }
        private void HandleStorageDeleteAll(NativeStorageDeleteAllRequest request)
        {
            new Thread(() =>
            {
                try
                {
                    _Storage.DeleteAll();
                    _WebViewMessagingInterface.MessageJavaScript(new NativeStorageDeleteAllResponse(request.Ticket));
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }).Start();
        }/*
        private void HandleDownloadFile(NativeDownloadFileRequest request)
        {
            new Thread(() =>
            {
                try
                {
                    Action doDownload = FileDownloader.Instance.DownloadFile(
                        request.ThroughServerReceiveUrl, request.FileName, 
                        out string directoryPath);
                    _WebViewMessagingInterface.MessageJavaScript(new NativeDownloadFileResponse(directoryPath, request.Ticket));
                    doDownload();
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }).Start();
        }*/
        private void NativeOpenDirectory(NativeOpenDirectoryRequest request)
        {
            new Thread(() =>
            {
                try
                {
                    _OpenDirectoryInExternalApp(request.DirectoryPath);
                }
                catch (Exception ex)
                {
                    Logs.Default.Error(ex);
                }
            }).Start();
        }
        private static TMessage Deserialize<TMessage>(MessageEventArgs e) {
            return Json.Deserialize<TMessage>(e.Message);
        }
    }
}