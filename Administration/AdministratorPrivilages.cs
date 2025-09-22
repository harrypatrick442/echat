namespace Chat
{
    [Flags]
    public enum AdministratorPrivilages
    {
        None =0,
        ChangeRoomPicture = 1,
        RenameRoom = 2,
        ChangeRoomVisibility = 4,
        ModifyRoomTags = 8,
        EditAdministrators = 16,
        BanUsers = 32,
        TimeoutUsers = 64,
        DeleteMessages= 128,
        Reserved2= 256,
        Reserved3= 512,
        Reserved4= 1024,
        Reserved5= 2048,
        Reserved6= 4096,
        Reserved7= 8192,
        Reserved8= 16384,
        All = 32767
    }
}