namespace Users.Enums
{
    [Flags]
    public enum Gender
    {
        NotDisclosed = 0,
        Woman = 1,
        Man = 2,
        Transgender = 4,
        NonBinary = 8,
        PreferNotToSay = 16
    }
}