namespace MultimediaCore
{
    public static class UserMultimediaConstrainer
    {
        public static string Description(string value)
        {
            if (value == null) 
                return null;
            if (value.Length < GlobalConstants.Lengths.MAX_USER_MULTIMEDIA_DESCRIPTION_LENGTH)
                return value;
            return value.Substring(0, GlobalConstants.Lengths.MAX_USER_MULTIMEDIA_DESCRIPTION_LENGTH);
        }
    }
}