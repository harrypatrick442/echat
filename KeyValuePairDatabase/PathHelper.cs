
using System.Text;

namespace KeyValuePairDatabases
{
    public static class PathHelper
    {
        public static string GetDirectoryFromIdentifier(string identifier, DirectoryInfo directoryInfoRoot, int nCharactersEachLevel)
        {
            int i = 0;
            int length = identifier.Length;
            return GetDirectoryFromIdentifier(directoryInfoRoot, nCharactersEachLevel, identifier, ref i, length);
        }
        public static void GetDirectoryPathAndFilePathFromIdentifier(string identifier, out string directoryPath, out string filePath,
            DirectoryInfo directoryInfoRoot, int nCharactersEachLevel, string extension)
        {
            int i = 0;
            int length = identifier.Length;
            directoryPath = GetDirectoryFromIdentifier(directoryInfoRoot, nCharactersEachLevel, identifier, ref i, length);
            filePath =  GetFileNameFromIdentifier_ContinuationOfGetDirectoryFromIdentifier(identifier, directoryPath, i, length, extension);
            
        }
        public static string GetDirectoryFromIdentifier(DirectoryInfo directoryInfoRoot, int nCharactersEachLevel, string identifier, ref int i, int length)
        {
            string path = directoryInfoRoot.FullName + Path.DirectorySeparatorChar;
            while (i < length - nCharactersEachLevel)
            {
                int charsIndex = 0;
                while (charsIndex < nCharactersEachLevel)
                {
                    char c = identifier[i++];
                    path += c;
                    charsIndex++;
                }
                path += Path.DirectorySeparatorChar;
            }
            return path;
        }
        public static string GetFileNameFromIdentifier_ContinuationOfGetDirectoryFromIdentifier(string identifier, 
            string path, int i, int length, string extension) {
            while (i < length)
            {
                char c = identifier[i++];
                path += c;
            }
            path += extension;
            return path;
        }
        public static string EscapeIdentifier(string identifier) {
            string validCharacters = "!#$%&'()+0123456789;=@ABCDEFGHIJKLMNOPQRSTUVWXYZ[]^_`abcdefghijklmnopqrstuvwxyz{}~";
            foreach (char c in identifier)
            {
                if (!validCharacters.Contains(c)) {
                    return _EscapeIdentifier(validCharacters, identifier);
                }
            }
            return identifier;
        }
        private static string _EscapeIdentifier(string validCharacters, string identifier) {
            StringBuilder sb = new StringBuilder();
            foreach (char c in identifier)
            {
                if (c == '&')
                {
                    sb.Append("&&");
                    continue;
                }
                if (validCharacters.Contains(c))
                {
                    sb.Append(c);
                    continue;
                }
                sb.Append('&');
                sb.Append(((int)c).ToString("x"));
            }
            return sb.ToString();
        }
    }
}