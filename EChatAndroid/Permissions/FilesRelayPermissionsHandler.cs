using Core.Exceptions;
using Android;
using NativeAndroid.Interfaces;
using Native.Permissions;
using NativeAndroid.Permissions;

namespace EChatAndroid.Permissions{
    public class EChatPermissionsHandler<TActivity>: PermissionsHandler<TActivity>
            where TActivity : Activity, IRequestPermissionsResultSource
    {
        private static readonly DesiredPermission[] _DesiredPermissions =
                new DesiredPermission[]{
                    new DesiredPermission(Manifest.Permission.ReadExternalStorage,false),
                    //new DesiredPermission(Manifest.Permission.WriteExternalStorage,false),
                    new DesiredPermission(Manifest.Permission.Internet, false),
                    new DesiredPermission(Manifest.Permission.AccessNetworkState, true),
                    new DesiredPermission(Manifest.Permission.PostNotifications, true),
                    new DesiredPermission("android.permission.READ_CLIPBOARD", true)
                };
        private static EChatPermissionsHandler<TActivity> _Instance;
        public static EChatPermissionsHandler<TActivity> Initialize(TActivity activity)
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(EChatPermissionsHandler<TActivity>));
            _Instance = new EChatPermissionsHandler<TActivity>(activity);
            return _Instance;
        }
        public static EChatPermissionsHandler<TActivity> Instance { get
            {
                if(_Instance == null)
                    throw new NotInitializedException(nameof(EChatPermissionsHandler<TActivity>));
                return _Instance;
            } 
        }
        private TActivity _Activity;
        private EChatPermissionsHandler(TActivity activity):base(activity, _DesiredPermissions,"EChat") {
            _Activity = activity;
        }
    }

}