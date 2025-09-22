
using Chat;

namespace Chat
{
    public enum AdministratorsFailedReason
    {
        RoomDoesntExist = 1,
        NotAdministrator = 2,
        CannotEditAdministrators=3,
        ServerError=4,
        NoPrivilagesProvided = 5,
        NoOtherAdmins = 6,
        DontHavePrivilages = 7,
    }
}