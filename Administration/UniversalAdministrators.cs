using System.Runtime.Serialization;
namespace Chat
{
    [DataContract]
    public static class UniversalAdministrators
    {
        public static readonly HashSet<long> UserIds = new HashSet<long> { };
    }
}
