namespace UserRouting
{
    public class LocalUserRoutingTable<TEndpoint> where TEndpoint : class
    {
        private Dictionary<long, Dictionary<long, TEndpoint>> _MapUserIdToMapSessionIdToEndpoint = new Dictionary<long, Dictionary<long, TEndpoint>>();
        public void Add(long userId, long sessionId, TEndpoint endpoint)
        {
            lock (_MapUserIdToMapSessionIdToEndpoint)
            {
                if (!_MapUserIdToMapSessionIdToEndpoint.ContainsKey(userId))
                {
                    _MapUserIdToMapSessionIdToEndpoint.Add(userId,
                        new Dictionary<long, TEndpoint> { { sessionId, endpoint } });
                    return;
                }
                _MapUserIdToMapSessionIdToEndpoint[userId].Add(sessionId, endpoint);
            }
        }
        public void Remove(long userId, long sessionId)
        {
            lock (_MapUserIdToMapSessionIdToEndpoint)
            {
                if (!_MapUserIdToMapSessionIdToEndpoint.TryGetValue(userId, out Dictionary<long, TEndpoint> mapSessionIdToEndpoint))
                    return;
                mapSessionIdToEndpoint.Remove(sessionId);
                if (mapSessionIdToEndpoint.Any()) return;
                _MapUserIdToMapSessionIdToEndpoint.Remove(userId);
            }
        }
        public long[] GetSessionIdsNoLongerHasForUser(long userId, long[] sessionIdsThinksHas)
        {
            lock (_MapUserIdToMapSessionIdToEndpoint)
            {
                if(!_MapUserIdToMapSessionIdToEndpoint.TryGetValue(userId, out Dictionary<long, TEndpoint> mapSessionIdToEndpoint)) {
                    return sessionIdsThinksHas;
                }
                long[] sessionIdsHas = mapSessionIdToEndpoint.Keys.ToArray();
                return sessionIdsThinksHas.Where(sessionIdThinksHas => !sessionIdsHas.Contains(sessionIdThinksHas)).ToArray();
            }
        }
        public TEndpoint[] GetLocalEndpointsForUser(long userId)
        {
            lock (_MapUserIdToMapSessionIdToEndpoint)
            {
                if (_MapUserIdToMapSessionIdToEndpoint.TryGetValue(userId, out Dictionary<long, TEndpoint> mapSessionInfoToEndpoint))
                {
                    return mapSessionInfoToEndpoint.Values.ToArray();
                }
                return null;
            }
        }
        public Dictionary<long, TEndpoint> GetMapSessionIdToEndpointForUserId(long userId)
        {
            lock (_MapUserIdToMapSessionIdToEndpoint)
            {
                if (_MapUserIdToMapSessionIdToEndpoint.TryGetValue(userId, out Dictionary<long, TEndpoint> mapSessionInfoToEndpoint))
                {
                    return mapSessionInfoToEndpoint.ToDictionary(p => p.Key, p => p.Value);
                }
                return null;
            }
        }
        public TEndpoint GetEndpoint(long userId, long sessionId)
        {
            lock (_MapUserIdToMapSessionIdToEndpoint)
            {
                if (_MapUserIdToMapSessionIdToEndpoint.TryGetValue(userId, out Dictionary<long, TEndpoint> mapSessionInfoToEndpoint))
                {
                    mapSessionInfoToEndpoint.TryGetValue(sessionId, out TEndpoint endpoint);
                    return endpoint;
                }
                return null;
            }
        }
        public TEndpoint[] GetEndpointsForUserIds(IEnumerable<long> userIds, out long[] userIdsDidntHave)
        {
            userIdsDidntHave = null;
            if (userIds == null) return null;
            List<TEndpoint> endpoints = new List<TEndpoint>();
            var userIdsDidntHaveList = new List<long>();
            lock (_MapUserIdToMapSessionIdToEndpoint)
            {
                foreach (long userId in userIds)
                {
                    if (!_MapUserIdToMapSessionIdToEndpoint.TryGetValue(userId, out Dictionary<long, TEndpoint> mapSessionInfoToEndpoint))
                    {
                        userIdsDidntHaveList.Add(userId);
                        continue;
                    }
                    endpoints.AddRange(mapSessionInfoToEndpoint.Values);
                }
            }
            userIdsDidntHave = userIdsDidntHaveList?.ToArray();
            return endpoints.GroupBy(endpoint => endpoint).Select(g => g.First()).ToArray();
        }
    }
}