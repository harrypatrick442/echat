using Database;
using Location.Interfaces;
using LocationCore;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LocationDatabase
{
    public class SqlLiteQuadrantsLocalDatabase : IQuadrantsLocalDatabase
    {
        private LocalSQLite[] _DatabaseForEachLevel;
        public SqlLiteQuadrantsLocalDatabase(string rootDirectory, int nLevels) {
            Directory.CreateDirectory(rootDirectory);
            _DatabaseForEachLevel = new LocalSQLite[nLevels];
            for (var i = 0; i < nLevels; i++)
            {
                string filePath = Path.Combine(rootDirectory, $"quads_{i}.sqlite");
                LocalSQLite localSqlite = new LocalSQLite(filePath, false);
                _DatabaseForEachLevel[i] = localSqlite;
                localSqlite.UsingConnection((connection) =>
                {
                    using (SqliteCommand command = new SqliteCommand(
                        "CREATE TABLE IF NOT EXISTS tblQuadrants (quadrant INTEGER NOT NULL, id INTEGER NOT NULL, lat REAL NOT NULL, lng REAL NOT NULL);"
                        + " CREATE INDEX IF NOT EXISTS indexQuadrant ON tblQuadrants (quadrant);"
                        + " CREATE INDEX IF NOT EXISTS indexId ON tblQuadrants (id);",
                        connection))
                    {
                        command.ExecuteNonQuery();
                    }
                });
            }
        }
        public void Delete(long id, int[] levels)
        {
            foreach (int level in levels) {
                _DatabaseForEachLevel[level].UsingConnection((connection) => {
                    using (SqliteCommand command = new SqliteCommand(
                        "DELETE FROM tblQuadrants WHERE id = @id;",
                        connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@id", id));
                        command.ExecuteNonQuery();
                    }
                });
            }
        }
        public Quadrant[] Get(LevelQuadrantPair[] levelQuadrantPairs)
        {
            Dictionary<long, Quadrant> mapIdToQuadrant = new Dictionary<long, Quadrant>();
            foreach (LevelQuadrantPair levelQuadrantPair in levelQuadrantPairs)
            {
                _DatabaseForEachLevel[levelQuadrantPair.Level].UsingConnection((connection) =>
                {
                    using (SqliteCommand command = new SqliteCommand(
                        "SELECT id, lat, lng FROM tblQuadrants WHERE quadrant = @quadrant;",
                        connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@quadrant", levelQuadrantPair.Quadrant));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long id = reader.GetInt64(0);
                                if (mapIdToQuadrant.ContainsKey(id))
                                    continue;
                                mapIdToQuadrant.Add(id, new Quadrant(id, reader.GetDouble(1), reader.GetDouble(2)));
                            }
                        }
                    }
                });
            }
            return mapIdToQuadrant.Values.ToArray();
        }

        public QuadrantNEntries[] GetNEntries(int level, long[] quadrants, bool withLatLng)
        {
            return withLatLng? GetNEntriesWithLatLng(level, quadrants):GetNEntriesWithoutLatLng(level, quadrants);
        }
        public QuadrantNEntries[] GetNEntriesWithLatLng(int level, long[] quadrants)
        {
            List<QuadrantNEntries> results = new List<QuadrantNEntries>();
            foreach (long quadrant in quadrants)
            {
                _DatabaseForEachLevel[level].UsingConnection((connection) =>
                {

                    /**/
                    string sql =
                        "WITH forCount AS " +
                        "   (SELECT Count(*) as count " +
                            "FROM tblQuadrants " +
                            "WHERE quadrant = @quadrant), " +
                        "forLatLngAverage AS " +
                        "   (SELECT AVG(lat) as latAvg, AVG(lng) as lngAvg, quadrant " +
                            "FROM tblQuadrants " +
                            "WHERE quadrant = @quadrant " +
                            "LIMIT 40) " +
                        "SELECT count, latAvg, lngAvg " +
                        "FROM forCount " +
                        "LEFT OUTER JOIN forLatLngAverage";
                    using (SqliteCommand command = new SqliteCommand(
                        sql,
                        connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@quadrant", quadrant));
                        using (var reader = command.ExecuteReader()) {
                            if (reader.Read())
                            {
                                int count = reader.GetInt32(0);
                                if (count > 0)
                                {
                                    results.Add(new QuadrantNEntries(count, reader.GetDouble(1), reader.GetDouble(2), quadrant));
                                }
                            }
                        }
                    }
                });
            }
            return results.ToArray();
        }
        public QuadrantNEntries[] GetNEntriesWithoutLatLng(int level, long[] quadrants)
        {
            List<QuadrantNEntries> results = new List<QuadrantNEntries>();
            foreach (long quadrant in quadrants)
            {
                _DatabaseForEachLevel[level].UsingConnection((connection) =>
                {

                    /**/
                    string sql = "SELECT Count(*) FROM tblQuadrants" +
                        " where quadrant = @quadrant";
                    using (SqliteCommand command = new SqliteCommand(
                        sql, 
                        connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@quadrant", quadrant));
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int count = reader.GetInt32(0);
                                if (count > 0)
                                {
                                    results.Add(new QuadrantNEntries(count, quadrant));
                                }
                            }
                        }
                    }
                });
            }
            return results.ToArray();
        }

        public void Set(long id, double lat, double lng, LevelQuadrantPair[] levelQuadrantPairs)
        {
            foreach (LevelQuadrantPair levelQuadrantPair in levelQuadrantPairs) {
                _DatabaseForEachLevel[levelQuadrantPair.Level].UsingConnection((connection) => {
                    using (SqliteCommand command = new SqliteCommand(
                        "INSERT INTO tblQuadrants (quadrant,id,lat,lng) VALUES(@quadrant,@id,@lat,@lng);",
                        connection))
                    {
                        command.Parameters.Add(new SqliteParameter("@id", id));
                        command.Parameters.Add(new SqliteParameter("@quadrant", levelQuadrantPair.Quadrant));
                        command.Parameters.Add(new SqliteParameter("@lat", lat));
                        command.Parameters.Add(new SqliteParameter("@lng", lng));
                        command.ExecuteNonQuery();
                    }
                });
            }
        }
    }
}
