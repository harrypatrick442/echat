using JSON;
using Core.Exceptions;
using Core.Ticketing;
using Core.Ids;
using Logging;
using InterserverComs;
using Nodes;
using Constants;

namespace KeyValuePairDatabases
{
    public class InternalNode
    {
        public struct(long startIndex, FileAccessor fileAccessor, )/
             [from pointer 8 bytes]
             *[is leaf 1 byte]
             *[nKeys 4 bytes] Can calculate start of values from this
             * [k1 (keyNBytes length)]
             * [k2 (keyNBytes length)]
             * [k3 (keyNBytes length)]
             * [v1 pointer 8 bytes]
             * [v2 pointer 8 bytes]
             * [v3 pointer 8 bytes]
             * [v4 pointer 8 bytes]
    }
}