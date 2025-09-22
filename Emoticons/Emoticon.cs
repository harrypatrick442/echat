namespace Emoticons
{
    public class Emoticon
    {
        public long Id { get; set; }
        public string MultimediaToken { get; protected set; }
        public string RelativePath { get; protected set; }
        public string[] Matches { get; protected set; }
        public string Description { get; protected set; }
        public EmoticonCategory Category { get; protected set; }
        public Emoticon(long id, string multimediaToken,
            string relativePath, string[] matches, string description, EmoticonCategory category)
        {
            Id = id;
            MultimediaToken = multimediaToken;
            RelativePath = relativePath;
            Matches = matches;
            Description = description;
            Category = category;
        }
    }
}
