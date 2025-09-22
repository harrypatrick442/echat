namespace MultimediaServerCore.Enums
{
    public enum MultimediaFailedReason
    {
        TokenInvalid = 1,
        SlowDown = 2,
        ServerError = 3,
        FileTooLarge=4,
        FileTypeNotSupported=5,
        Processing=6,
        Permissions=7
    }
}
