using Core.DTOs;
using Core.Enums;
using Core.Assets;
using Core.Exceptions;
using KeyValuePairDatabases;

namespace Core.DAL
{
    public class DalClientStates
    {
        private static DalClientStates _Instance;
        public static DalClientStates Instance
        {
            get
            {
                if (_Instance == null) throw new NotInitializedException(nameof(DalClientStates));
                return _Instance;
            }
        }
        public static DalClientStates Initialize()
        {
            if (_Instance != null)
                throw new AlreadyInitializedException(nameof(DalClientStates));
            _Instance = new DalClientStates();
            return _Instance;
        }
        public KeyValuePairDatabaseMesh<DeviceStates> _UserIdToDeviceStateKeyValuePairDatabse;
        private DalClientStates()
        {
            _UserIdToDeviceStateKeyValuePairDatabse
            = new KeyValuePairDatabaseMesh<DeviceStates>(
                DatabaseIdentifier.UserIdToClientState.Int(),
                rootDirectory: Paths.UserIdToDeviceStatesDatabaseDirectory,
                nCharactersEachLevel: 2,
                extension: ".json",
                new IdentifierLock(),
                NodeAssignedIdRangesIdentifierToNodeId.Instance,
                inMemoryOnlyAllowedElseAlwaysWriteToDiskToo: false);
        }
        public DeviceState GetDeviceState(long userId, string deviceIdentifier,
            out bool foundForSpecifiedDevice)
        {
            foundForSpecifiedDevice = false;
            DeviceStates deviceStates = _UserIdToDeviceStateKeyValuePairDatabse.Get(userId);
            if (deviceStates?.Entries == null) return null;
            DeviceState deviceState = null;
            if (deviceStates?.Entries == null ||
                string.IsNullOrEmpty(deviceIdentifier))
                return deviceStates.Entries.FirstOrDefault();
            deviceState = deviceStates.Entries
                    .Where(entry => entry.DeviceIdentifier == deviceIdentifier)
                    .FirstOrDefault(); ;
            if (deviceState != null)
            {
                foundForSpecifiedDevice = true;
                return deviceState;
            }
            deviceState = deviceStates.Entries
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefault();
            return deviceState;
        }
        public void SetDeviceState(long userId, DeviceState newDeviceState)
        {
            _UserIdToDeviceStateKeyValuePairDatabse.ModifyWithinLock(userId, (deviceStates) =>
            {
                if (deviceStates == null)
                {
                    return new DeviceStates( newDeviceState);
                }
                deviceStates.UpdateForDeviceIdentifier(newDeviceState);
                return deviceStates;
            });
        }
    }
}