using System.Text;
using System.Text.RegularExpressions;

namespace EmoticonPreparation
{
    public static class FixedEmoticonPreparation
    {
        private const string SMILEYS_DIRECTORY_PATH = "C:\\repos\\snippets\\client\\src\\emoticons\\smileys";
        private static readonly Regex GET_CODE_FROM_FILE_NAME = new Regex("emoji_u([0-9a-z]+)");
        public static void Run() {
            string directoryPath = SMILEYS_DIRECTORY_PATH;
            StringBuilder sb = new StringBuilder();
            foreach (string filePath in Directory.GetFiles(directoryPath))
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
                string[] splitsOnSpaces = fileNameWithoutExtension.Split(',');
                Match match = GET_CODE_FROM_FILE_NAME.Match(splitsOnSpaces[0]);
                string description = splitsOnSpaces.Length > 1 ? splitsOnSpaces[1] : null;
                string keywords = splitsOnSpaces.Length > 2 ? splitsOnSpaces[2] : null;
                description = description?.Replace("  ", " ");
                keywords = keywords?.Replace("  ", " ");
                if (!match.Success)
                {
                    throw new Exception();
                }
                string code = match.Groups[1].Value;
                string svgFilePath = Path.Combine(directoryPath, $"emoji_u{code}");
                sb.Append("['");
                sb.Append(code);
                if (description == null)
                {
                    sb.Append("'],");
                    continue;
                }
                sb.Append("','");
                sb.Append(description);
                if (keywords == null)
                {

                    sb.Append("'],");
                    continue;
                }
                sb.Append("','");
                sb.Append(keywords);
                sb.Append("'],");
            }
            string str = sb.ToString();
        }
    }
}
