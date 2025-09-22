using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Authentication.Enums
{
    public delegate long DelegateCreateNewUser(bool guest, string username);
}
