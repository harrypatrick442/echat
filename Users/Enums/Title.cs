namespace Users.Enums
{
    public enum Title
    {
        Mr = 1,
        Mrs = 2,
        Miss = 3,
        Ms = 4
    }
    public static class TitleHelper
    {
        public static string GetString(this Title title) {
            return Enum.GetName(typeof(Title), title);
        }
    }
}