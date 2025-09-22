namespace UsersEnums
{
    [Flags]
    public enum VisibleTo
    {
        None=0,
        MeOnly=1,
        Friend=AssociateType.Friend,
        Family=AssociateType.Family,
        Collegue=AssociateType.Collegue,
        Public=16
    }
}