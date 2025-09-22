using Logging;
using System.Text;

namespace FileServerBase
{
    public class IndexFileWithInserts
    {
        private string[] _Chunks;
        private string[] _Placeholders;
        public IndexFileWithInserts(string filePath, params string[] placeholders) {
            Logs.Default.Info("IndexFileWithInserts");
            _Placeholders = placeholders;
            string content = File.ReadAllText(filePath);
            List<Tuple<int, string>> indexAndPlaceholder = new List<Tuple<int, string>>();
            foreach (string placeholder in placeholders) {
                int index = content.IndexOf(placeholder);
                if (index < 0)
                    throw new Exception($"Placeholder \"{placeholder}\" was not found");
                indexAndPlaceholder.Add(new Tuple<int, string>(index, placeholder));

            }
            List<string> chunks = new List<string>();
            int startIndex = 0;
            foreach (var tuple in indexAndPlaceholder.OrderBy(t => t.Item1))
            {
                Logs.Default.Info("A placeholder: " + tuple.Item2);
                string chunk = content.Substring(startIndex, tuple.Item1 - startIndex);
                startIndex = tuple.Item1 + tuple.Item2.Length;
                chunks.Add(chunk);
            }
            if (startIndex < content.Length) {
                string finalChunk = content.Substring(startIndex, content.Length - startIndex);
                chunks.Add(finalChunk);
            }
            _Chunks = chunks.ToArray();
        }
        public byte[] GetBytes(params string[] value) {
            StringBuilder sb = new StringBuilder();
            if(value.Length>_Chunks.Length)
            {
                string placeholders = string.Join(',', _Placeholders.Select(p => $"\"{p}\""));
                throw new Exception($"Too many values were provided ({value.Length}). The provided placeholders were {placeholders}");
            }
            for (int i = 0; i < _Chunks.Length; i++) {
                sb.Append(_Chunks[i]);
                if (i < value.Length) {
                    sb.Append(value[i]);
                }
            }
            return System.Text.Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}