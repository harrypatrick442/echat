
namespace KeyValuePairDatabases
{
    public enum Operation
    {
        GetOutsideLock,
        GetThenDeleteWithinLock,
        ModifyWithinLock,
        Get,
        Set,
        Delete,
        Has
    }
}