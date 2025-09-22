using Core.Events;
using Core.Interfaces;
using Core.InterserverComs;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace UserRouting
{
    public class ToPushWhenNodesReconnect
    {
        private Dictionary<int, ToPushWhenNodeReconnects> _MapNodeIdToPushWhenNodeReconnects = 
            new Dictionary<int, ToPushWhenNodeReconnects>();
        public ToPushWhenNodesReconnect()
        {
        }
        public void Add(int nodeId, long userId)
        {
            ToPushWhenNodeReconnects toPushWhenNodeReconnects;
            lock (_MapNodeIdToPushWhenNodeReconnects)
            {
                if (!_MapNodeIdToPushWhenNodeReconnects.TryGetValue(nodeId,
                    out toPushWhenNodeReconnects))
                {
                    toPushWhenNodeReconnects = new ToPushWhenNodeReconnects();
                    _MapNodeIdToPushWhenNodeReconnects[nodeId] = toPushWhenNodeReconnects;
                }
            }
            toPushWhenNodeReconnects.Add(userId);
        }
        public void AddRange(int nodeId, long[] userIds)
        {
            GetOrCreate(nodeId).AddRange(userIds);
        }
        public bool TakeBatchOfUserIds(int nodeId,
                    int maxUserIdsToSendAtOnce, out long[] userIds)
        {

            ToPushWhenNodeReconnects toPushWhenNodeReconnects;
            lock (_MapNodeIdToPushWhenNodeReconnects)
            {
                if (!_MapNodeIdToPushWhenNodeReconnects
                    .TryGetValue(nodeId,
                    out toPushWhenNodeReconnects))
                {
                    userIds = null;
                    return false;
                }
                return toPushWhenNodeReconnects.TakeBatchOfUserIds(maxUserIdsToSendAtOnce, out userIds);
            }
        }
        private ToPushWhenNodeReconnects GetOrCreate(int nodeId) {

            lock (_MapNodeIdToPushWhenNodeReconnects)
            {
                ToPushWhenNodeReconnects toPushWhenNodeReconnects;
                if (!_MapNodeIdToPushWhenNodeReconnects.TryGetValue(nodeId,
                    out toPushWhenNodeReconnects))
                {
                    toPushWhenNodeReconnects = new ToPushWhenNodeReconnects();
                    _MapNodeIdToPushWhenNodeReconnects[nodeId] = toPushWhenNodeReconnects;
                }
                return toPushWhenNodeReconnects;
            }
        }
    }
}
