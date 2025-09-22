using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Webkit;
using Core.Enums;
using EChatAndroid.Clipboard;
using EChatAndroid.Permissions;
using EChatNative;
using EChatNative.API;
using Logging;
using Native.WebViewInterface;
using NativeAndroid;
using NativeAndroid.Clipboards;
using NativeAndroid.Delegates;
using NativeAndroid.Interfaces;
using NativeAndroid.Networks;
using NativeAndroid.Permissions;
using NativeAndroid.Storages;
using Shutdown;
using System.Timers;
using static Java.Util.Jar.Attributes;
using Timer = System.Timers.Timer;
using Uri = Android.Net.Uri;
namespace EChatAndroid
{
    [Activity(Name="com.onestonesolutions.echat.MainActivity",
        Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : Activity, IActivityResultSource, INetworkStateListener, NativeAndroid.Interfaces.IRequestPermissionsResultSource
    {
        private const int DELAY_LOADING_BEFORE_SHOW_OFFLINE = 6000;
        private const bool ALWAYS_SHOW_WEBVIEW_TO_DEBUG = false;
        private Dictionary<int, DelegateHandleActivityResult> _MapRequestCodeCodeToHandleActivityResult = new Dictionary<int, DelegateHandleActivityResult>();
        private Dictionary<int, DelegateHandleRequestPermissionsResult> _MapRequestCodeCodeToHandleRequestPermissionsResult = new Dictionary<int, DelegateHandleRequestPermissionsResult>();
        private WebView _WebView;
        private ImageButton _ReloadButton;
        private ImageView _Spinner, _OfflineImage;
        private RelativeLayout _Mask;
        private Timer _TimerOffline;
        private PermissionsHandler _PermissionsHandler;
        private bool _Online = true;
        private bool _FailedToLoad = false;
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidEnvironmentUnhandledExceptionRaiser;
            this.ActionBar.Hide();
            ShutdownManager shutdownManager = ShutdownManager.Initialize(
                this.FinishActivity,
                () => null);
            Logs.Initialize(null);
            LogServerClient logServerClient = LogServerClient.Initialize(PlatformHelper.GetPlatform(), Core.Enums.Project.EChatClient, -1);
            Logs.Add(logServerClient);
            FileDownloader.Initializer(GetDownloadsDirectory());
            EChatClipboardWatcher.Initialize(
                AndroidClipboardWatcher.Initialize(this));
            SetContentView(Resource.Layout.activity_main);
            _WebView = FindViewById<WebView>(Resource.Id.WebView)!;
            _ReloadButton = FindViewById<ImageButton>(Resource.Id.ReloadButton)!;
            _Spinner = FindViewById<ImageView>(Resource.Id.Spinner)!;
            _Mask = FindViewById<RelativeLayout>(Resource.Id.Mask)!;
            _OfflineImage = FindViewById<ImageView>(Resource.Id.Offline)!;

            if(ALWAYS_SHOW_WEBVIEW_TO_DEBUG)
                ShowAsOnline();


            _TimerOffline = new Timer(DELAY_LOADING_BEFORE_SHOW_OFFLINE);
            _TimerOffline.Enabled = true;
            _TimerOffline.AutoReset = false;
            _TimerOffline.Elapsed += TimedOutLoading;
            WebView.SetWebContentsDebuggingEnabled(true);
            _PermissionsHandler = EChatPermissionsHandler<MainActivity>.Initialize(this);
            IWebViewHandler webViewHandler =
                new AndroidWebViewHandler<MainActivity>(this, _WebView);
            EChatWebViewInterface.Initialize(
                new WebViewMessagingInterface(webViewHandler),
                OpenDirectoryInExternalApp, 
                FinishedLoading, Reload,
                _PermissionsHandler,
                Storage.Initialize(this)
            );
            _ReloadButton.Click += ClickedReloadButton;
            _Spinner.Visibility = Android.Views.ViewStates.Gone;
            _OfflineImage.Visibility = Android.Views.ViewStates.Gone;
            string packageName = ApplicationContext.PackageName;
            DoPermissions();
            Load();

        }
        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
        }
        protected override void OnResume()
        {
            base.OnResume();
            if (_FailedToLoad) {
                Load();
                return;
            }
            EChatWebViewInterface.Instance.SendPermissionsUpdate(_PermissionsHandler.HasAllRequired());
        }
        private void DoPermissions() {
            _PermissionsHandler.Handle((hasAllRequired, requestPermissionResults) => {
                EChatWebViewInterface.Instance.SendPermissionsUpdate(hasAllRequired);
            });
        }
        private void TimedOutLoading(object sender, ElapsedEventArgs e)
        {
            this.RunOnUiThread(() =>
            {
                _FailedToLoad = true;
                if (ALWAYS_SHOW_WEBVIEW_TO_DEBUG)
                    ShowOffline();
            });
        }
        private void ShowOffline()
        {
            _ReloadButton.ClearAnimation();
            _Spinner.ClearAnimation();
            _Spinner.Visibility = ViewStates.Gone;
            _ReloadButton.Visibility = ViewStates.Visible;
            _Mask.Visibility = ViewStates.Visible;
            _OfflineImage.Visibility = ViewStates.Visible;
        }
        private void ShowAsOnline()
        {
            _Spinner.ClearAnimation();
            _Spinner.Visibility = ViewStates.Gone;
            _OfflineImage.Visibility = ViewStates.Gone;
            _ReloadButton.ClearAnimation();
            _ReloadButton.Visibility = ViewStates.Visible;
            _WebView.Visibility = ViewStates.Visible;
            _Mask.Visibility = ViewStates.Gone;
        }
        private void ClickedReloadButton(object sender, EventArgs e)
        {
            Reload();
        }
        private void Reload()
        {
            //Intent intent = new Intent(Intent.ActionView, Uri.Parse("https://filesrelay.com/d/c0587c8349334448987343153cf553b3"));
            //StartActivity(intent);
            Load();
            _ReloadButton.StartAnimation(CreateRotationAnimation());
        }
        private void Load()
        {
            StartedLoading();
            _TimerOffline.Start();
#if DEBUG
            _WebView.LoadUrl("http://10.0.2.2:3000");
            //_WebView.LoadUrl("https://dev.e-chat.live");
#else
            _WebView.LoadUrl("https://e-chat.live");
#endif
        }
        private void FinishedLoading()
        {
            this.RunOnUiThread(() =>
            {
                _FailedToLoad = false;
                _TimerOffline.Stop();
                ShowAsOnline();
                DoPermissions();

            });
            new Thread(() =>
            {
                try
                {
                    EChatWebViewInterface.Instance
                    .SendPermissionsUpdate(_PermissionsHandler.HasAllRequired());
                }
                catch (Exception ex) {
                    Logs.Default.Error(ex);
                }
            }).Start();
        }
        private void StartedLoading()
        {
            _Spinner.Visibility = Android.Views.ViewStates.Visible;
            _Spinner.StartAnimation(CreateRotationAnimation());
        }
        private Animation CreateRotationAnimation() {
            RotateAnimation rotate = new RotateAnimation(0, 360, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
            rotate.Duration = 1200;
            rotate.RepeatMode = RepeatMode.Restart;
            rotate.RepeatCount = int.MaxValue;
            rotate.Interpolator = new LinearInterpolator();
            return rotate;
        }
        private string GetDownloadsDirectory() {
            string externalStorageDirectory = Android.OS.Environment.ExternalStorageDirectory!.AbsolutePath;
            string downloadsDirectory = Android.OS.Environment.DirectoryDownloads!;
            return System.IO.Path.Combine(externalStorageDirectory, downloadsDirectory);
        }
        public void OpenDirectoryInExternalApp(string path)
        {
            if (path == null) return;
            Intent intent = new Intent(Intent.ActionGetContent);
            Android.Net.Uri? uri = Android.Net.Uri.Parse(path);
            if (uri == null) return;
            intent.SetDataAndType(uri, "*/*");
            StartActivity(Intent.CreateChooser(intent, "Open folder"));
        }

        public void NetworkAvailable()
        {
            this.RunOnUiThread(() => {
                if (_Online) return;
                _Online = true;
                Load();
            });
        }

        public void NetworkUnavailable()
        {
            this.RunOnUiThread(() => {
                if (!_Online) return;
                _Online = false;
            });
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (!_MapRequestCodeCodeToHandleActivityResult.TryGetValue(requestCode, out DelegateHandleActivityResult handle))
                return;
            _MapRequestCodeCodeToHandleActivityResult.Remove(requestCode);
            handle(requestCode, resultCode, data);
        }
        public override void OnRequestPermissionsResult(int requestCode,
            string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            if (!_MapRequestCodeCodeToHandleRequestPermissionsResult.TryGetValue(requestCode, out DelegateHandleRequestPermissionsResult handle))
                return;
            _MapRequestCodeCodeToHandleRequestPermissionsResult.Remove(requestCode);
            handle(requestCode, permissions, grantResults);
        }
        public void AddHandler(int requestCode, DelegateHandleActivityResult handler)
        {
            _MapRequestCodeCodeToHandleActivityResult.Add(requestCode, handler);
        }
        public void AddHandler(int requestCode, DelegateHandleRequestPermissionsResult handler)
        {
            _MapRequestCodeCodeToHandleRequestPermissionsResult.Add(requestCode, handler);
        }
        private void OnAndroidEnvironmentUnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs unhandledExceptionEventArgs)
        {
            var newExc = new Exception("OnAndroidEnvironmentUnhandledExceptionRaiser", unhandledExceptionEventArgs.Exception);
            try
            {
                Logs.Default.Error(newExc);
            }
            catch{ 
                
            }
        }
    }
}