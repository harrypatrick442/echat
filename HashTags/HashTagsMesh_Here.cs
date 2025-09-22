using HashTags.DAL;
using HashTags.Messages;
using HashTags.Enums;

namespace HashTags
{
    public partial class HashTagsMesh
    {
        private ScopeIds[] SearchTags_Here(string tag, HashTagScopeTypes? scopeType, bool allowPartialMatches, int maxNEntries, out TagWithScopeIds[]? partialMatches)
        {
            return DalHashTags.Instance.Search(tag, scopeType, allowPartialMatches, maxNEntries, out partialMatches);
        }
        private string[] SearchToPredictTag_Here(string str, HashTagScopeTypes? scopeType, int maxNEntries)
        {
            return DalHashTags.Instance.SearchToPredictTag(str, scopeType, maxNEntries);
        }
        private void AddTags_Here(string[] tags, HashTagScopeTypes scopeType, long scopeId, long? scopeId2)
        {
            DalHashTags.Instance.AddTags(tags, scopeType, scopeId, scopeId2);
        }
        private void DeleteTags_Here(HashTagScopeTypes scopeType, long scopeId, long? scopeId2, string[] tags)
        {
            DalHashTags.Instance.Delete(scopeType, scopeId, scopeId2, tags);
        }
    }
}