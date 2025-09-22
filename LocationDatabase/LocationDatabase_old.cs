using KeyValuePairDatabases;
using LocationCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LocationDatabase
{
    public class LocationDatabase_old
    {
        private IdentifierLock _IdentifierLockSnippetId = new IdentifierLock();
        private KeyValuePairDatabase<Quadrant>[] _MapSpecificTo_QuadrantIdToQuadrantKeyValuePairDatabases;
        private KeyValuePairDatabase<SnippetQuadrants> _MapSpecificToSnippetId_SnippetIdToSnippetQuadrantsKeyValuePairDatabase;
        private int _NLevels;
        public LocationDatabase(int nLevels, string rootDirectory) {
            _NLevels = nLevels;
            _MapSpecificTo_QuadrantIdToQuadrantKeyValuePairDatabases = new KeyValuePairDatabase<Quadrant>[nLevels];
            for (var i = 0; i < nLevels; i++) {
                _MapSpecificTo_QuadrantIdToQuadrantKeyValuePairDatabases[i] = new KeyValuePairDatabase<Quadrant>(Path.Combine(rootDirectory, i.ToString()), 2, ".json", new IdentifierLock());
            }
            _MapSpecificToSnippetId_SnippetIdToSnippetQuadrantsKeyValuePairDatabase = new KeyValuePairDatabase<SnippetQuadrants>(
                Path.Combine(rootDirectory, "snippetIdToQuadrants"), 2, ".json", new NoIdentifierLock());
        }
        public long[] GetSnippetIds(LatLng latLng, double radiusKm, long specificToSnippetId, int nLevels = 4) {
            LevelQuadrantPair[] levelAndQuadrants = QuadrantsHelper.GetQuadrantsForLatLngAndRadius(latLng, radiusKm, nLevels);
            HashSet<long> snippetIds = new HashSet<long>();
            foreach (LevelQuadrantPair levelAndQuadrant in levelAndQuadrants) {
                if (levelAndQuadrant.Level < 0 || levelAndQuadrant.Level >= _MapSpecificTo_QuadrantIdToQuadrantKeyValuePairDatabases.Length) continue;
                Quadrant quadrant = _MapSpecificTo_QuadrantIdToQuadrantKeyValuePairDatabases[levelAndQuadrant.Level].Get(GetSpecificTo_QuadrantIdentifier(specificToSnippetId, levelAndQuadrant.Quadrant));
                foreach (long snippetId in quadrant.SnippetIds) {
                    if (snippetIds.Contains(snippetId)) continue;
                    snippetIds.Add(snippetId);
                }
            }
            return snippetIds.ToArray();
        }
        public void Set(long specificToSnippetId, long snippetId, LatLng latLng) {
            long[] quadrantAtEachLevel = QuadrantsHelper.GetQuadrantsForLatLng(latLng, _NLevels);
            Set(specificToSnippetId, new SnippetQuadrants(snippetId, quadrantAtEachLevel));
        }
        public void Set(long specificToSnippetId, SnippetQuadrants snippetQuadrants) {
            long snippetId = snippetQuadrants.SnippetId;
            string identifier = GetSpecificTo_SnippetIdIdentifier(specificToSnippetId, snippetId);
            _IdentifierLockSnippetId.LockForWrite(identifier, () => {
                //Done in this order to prevent leaking quadrants without an entry mapping them all in _MapSnippetIdToQuadrantsKeyValuePairDatabase
                SnippetQuadrants existingSnippetQuadrants = _MapSpecificToSnippetId_SnippetIdToSnippetQuadrantsKeyValuePairDatabase.Get(identifier);
                if(existingSnippetQuadrants!=null)
                    _DeleteQuadrants(specificToSnippetId, existingSnippetQuadrants);
                _MapSpecificToSnippetId_SnippetIdToSnippetQuadrantsKeyValuePairDatabase.Set(identifier, snippetQuadrants);
                for (int level = 0; level < snippetQuadrants.QuadrantAtEachLevel.Length; level++)
                {
                    long quadrantAtLevel = snippetQuadrants.QuadrantAtEachLevel[level];
                    _MapSpecificTo_QuadrantIdToQuadrantKeyValuePairDatabases[level]
                        .ModifyWithinLock(GetSpecificTo_QuadrantIdentifier(specificToSnippetId, quadrantAtLevel), (quadrant) =>
                    {
                        if (quadrant == null) quadrant = new Quadrant();
                        quadrant.AddSnippetId(snippetId);
                        return quadrant;
                    });
                }
            });
        }
        public void Delete(long specificToSnippetId, long snippetId)
        {
            string snippetIdIdentifier = snippetId.ToString();
            string identifier = GetSpecificTo_SnippetIdIdentifier(specificToSnippetId, snippetId);
            _IdentifierLockSnippetId.LockForWrite(identifier, () =>
            {
                //Done in this order to prevent leaking quadrants without an entry mapping them all in _MapSnippetIdToQuadrantsKeyValuePairDatabase
                SnippetQuadrants existingSnippetQuadrants = _MapSpecificToSnippetId_SnippetIdToSnippetQuadrantsKeyValuePairDatabase.Get(identifier);
                _DeleteQuadrants(specificToSnippetId, existingSnippetQuadrants);
                _MapSpecificToSnippetId_SnippetIdToSnippetQuadrantsKeyValuePairDatabase.Delete(identifier);
            });
        }
        private void _DeleteQuadrants(long specificToSnippetId, SnippetQuadrants snippetQuadrants)
        {
            for (int level = 0; level < snippetQuadrants.QuadrantAtEachLevel.Length; level++)
            {
                long quadrantAtLevel = snippetQuadrants.QuadrantAtEachLevel[level];
                _MapSpecificTo_QuadrantIdToQuadrantKeyValuePairDatabases[level].ModifyWithinLock(GetSpecificTo_QuadrantIdentifier(specificToSnippetId, quadrantAtLevel), (quadrant) =>
                {
                    if (quadrant == null) quadrant = new Quadrant();
                    quadrant.DeleteSnippetId(snippetQuadrants.SnippetId);
                    return quadrant;
                });
            }
        }
        private string GetSpecificTo_SnippetIdIdentifier(long specificToSnippetId, long snippetId)
        {
            return $"{specificToSnippetId}_{snippetId}";
        }
        private string GetSpecificTo_QuadrantIdentifier(long specificToSnippetId, long quadrantIdentifier)
        {
            return $"{specificToSnippetId}_{quadrantIdentifier}";
        }
    }
}
