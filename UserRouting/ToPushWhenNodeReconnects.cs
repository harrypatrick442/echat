using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace UserRouting
{
    public class ToPushWhenNodeReconnects
    {
        private HashSet<long> _UserIds = new HashSet<long>();
        public void Add(long userId)
        {
            lock (_UserIds)
            {
                if (_UserIds.Contains(userId)) return;
                _UserIds.Add(userId);
            }
        }
        public void AddRange(long[] userIds)
        {
            lock (_UserIds)
            {
                foreach (long userId in userIds)
                {
                    if (_UserIds.Contains(userId)) continue;
                    _UserIds.Add(userId);
                }
            }
        }
        public bool TakeBatchOfUserIds(int maxUserIdsToSendAtOnce,
            out long[] userIds)
        {
            lock (_UserIds) {
                userIds = _UserIds
                    .Take(maxUserIdsToSendAtOnce)
                    .ToArray();
                foreach (long userId in userIds)
                    _UserIds.Remove(userId);
                return userIds.Length>0;
            }
        }
    }
}
