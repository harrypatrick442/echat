namespace UserRouting
{
    public enum UserRoutingOperation
    {
        None,
        NonCoreUpdate,
        GetUserRoutingTableEntry,
        AddCore,
        RemoveCore,
        RemoveAsSessionsNoLongerExistToCoreMachine,
        BulkNonCoreUpdate
    }
}