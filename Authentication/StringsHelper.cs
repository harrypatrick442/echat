using Core.Limiters;

namespace Authentication
{
    public class StringsHelper
    {
        public static string NormalizeEmail(string email)
        {
            return email?.ToLower()?.Trim();
        }
        public static string NormalizePhone(string phone)
        {
            return phone?.ToLower()?.Trim()?.Replace(" ", "");//ToDo much more complicated than this and depends on country. I think country code should be in all numbers in database
        }
    }
}
