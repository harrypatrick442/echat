using JSON;
using Core.DTOs;
namespace Core.DAL
{
    public class DalAlarms
    {
        private static DalAlarms _Instance = new DalAlarms();
        public static DalAlarms Instance { get { return _Instance; } }

        /*
        public KeyValuePairOnDiskDatabase _FirstAtTimeKeyValuePairOnDiskDatabase = 
            new KeyValuePairOnDiskDatabase(Paths.AlarmsDatabaseDirectory,
            nCharactersEachLevel: 1,
            extension: ".json",
            new IdentifierLock());
        private IdSource _IdentifierSource = new IdSource(Paths.AlarmsIdentifierSourceJsonFilePath);
        /*to use a hybrid of the key value pair on disk database to iterate over relavent directories. Non volatile, organised and relatively efficient.*/

        private Json _NativeJsonParser = new Json();
        private DalAlarms() { }
        public AlarmsAtTime[]GetAlarmsBetweenTimes(long fromInclusive, long toExclusive){
            throw new NotImplementedException();
            /*
            int[] unitsFromInclusive = GetUnits(fromInclusive);
            int[] unitsToInclusive = GetUnits(toExclusive - 1);
            int index = 0;
            int fromLength = unitsFromInclusive.Length;
            int toLength = unitsToInclusive.Length;
            int shortestLength = fromLength < toLength ? fromLength : toLength;
            while (index < shortestLength) {
                if (unitsFromInclusive[index] != unitsToInclusive[index]) break;
                index++;
            }
            //124533454
            //1245733
            string sharedDirectoryPath = Paths.AlarmsDatabaseDirectory;
            for (int i = 0; i < index; i++)
                sharedDirectoryPath += unitsFromInclusive[i].ToString() + Path.DirectorySeparatorChar;
            string currentDirectoryPathIn = sharedDirectoryPath;
            while (index <) {
                string[] filePaths = Directory.GetFiles(currentDirectoryPathIn);


            }*/
        }
    }
}