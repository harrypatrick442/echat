using Core.Strings;

namespace HashTags
{
    public static class HashTagsHelper
    {
        public static IEnumerable<string> SplitStringIntoTags(string str) {
            if (string.IsNullOrEmpty(str)) return null;
            return NormalizeRemoveIllegalCharactersAndRemoveDuplicates(
                StringHelper.MultipleSplit(Configurations.HashTags.Delimiters, str)
            );
        }
        public static IEnumerable<string> NormalizeRemoveIllegalCharactersAndRemoveDuplicates(string[] tags) {
            return tags
                .Select(t => NormalizeRemoveIllegalCharacters(t))
                .Where(t=>t!=null).GroupBy(t=>t).Select(g=>g.First());
        }
        public static string? NormalizeRemoveIllegalCharacters(string? tag)
        {
            if (tag == null) return null;
            char[] chars = tag.ToLowerInvariant().Where(c => Configurations.HashTags.ALLOWED_CHARACTERS_HASH_SET.Contains(c)).ToArray();
            if (chars.Length < 1) return null;
            if (chars.Length > Configurations.HashTags.MAX_LENGTH) {
                return new string(chars, 0, Configurations.HashTags.MAX_LENGTH);
            }
            return new string(chars);
        }
        public static void CrossCompareTags(string[] requestTags, string[] currentTags,
            out string[]? tagsToRemove, out string[]? tagsToAdd)
        {
            if (currentTags == null)
            {
                tagsToRemove = null;
                tagsToAdd = requestTags == null ? null : NormalizeRemoveIllegalCharactersAndRemoveDuplicates(requestTags).ToArray();
                return;
            }
            if (requestTags == null)
            {
                tagsToRemove = currentTags == null ? null : NormalizeRemoveIllegalCharactersAndRemoveDuplicates(currentTags).ToArray();
                tagsToAdd = null;
                return;
            }
            HashSet<string> requestTagsSet = NormalizeRemoveIllegalCharactersAndRemoveDuplicates(requestTags).ToHashSet();
            HashSet<string> currentTagsSet = NormalizeRemoveIllegalCharactersAndRemoveDuplicates(currentTags).ToHashSet();
            tagsToRemove = currentTagsSet.Where(currentTag => !requestTagsSet.Contains(currentTag)).ToArray();
            tagsToAdd = requestTagsSet.Where(requestTag => !currentTagsSet.Contains(requestTag)).ToArray();
        }
    }
}