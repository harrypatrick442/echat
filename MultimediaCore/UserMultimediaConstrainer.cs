using Configurations;
using DependencyManagement;

namespace MultimediaCore
{
    public static class UserMultimediaConstrainer
    {
        public static string Description(string value)
        {
            if (value == null) 
                return null;
            int maxLength = DependencyManager.Get<Lengths>().MaxUserMultimediaDescriptionLength;
            if (value.Length < maxLength)
                return value;
            return value.Substring(0, maxLength);
        }
    }
}