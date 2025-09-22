namespace NotificationsCore
{
    public static class Initializer
    {
        public static void Initialize() {
            DalNotifications.Initialize();
            UserNotificationsMesh.Initialize();
        }
    }
}
