using Android;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Webkit;
using Constants;
using Core.Enums;
using EChatNative.API;
using Logging;
using static System.Net.Mime.MediaTypeNames;
using Uri = Android.Net.Uri;

namespace EChatAndroid
{
    [Activity(Name = "com.onestonesolutions.echat.HandleUrlActivity")]
    public class HandleUrlActivity : Activity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            try
            {
                /*
                    Uri? uri = Intent.Data;
                    if (uri == null) return;
                    string token = FilesRelayTokensHelper
                        .GetTokenFromPotentialUrl(uri.ToString()!);
                    if (token == null) return;
                    FilesRelayWebViewInterface.Instance.SendGotNewToken(token);
                */
            }
            catch (Exception ex)
            {
                Logs.Default.Error(ex);
            }
            finally
            {
                Finish();
            }
        }
    }
}