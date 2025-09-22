using System.Collections.Generic;

namespace GlobalConstants
{
    public class Nodes
    {
        public const int
            ECHAT_DEBUG = 4,
            CLIENT_TO_CLIENT_FILES_RELAY = 4,
            ECHAT_1 = 13,
            ECHAT_MOST_ACTIVE_ROOMS_MANAGER = 13,
            ECHAT_MOST_ACTIVE_ROOMS_MANAGER_DEBUG = 4,
            ECHAT_POPULAR_ROOMS_MANAGER = 13,
            ECHAT_POPULAR_ROOMS_MANAGER_DEBUG = 4,
            ECHAT_USERNAME_SEARCH = 13,
            ECHAT_USERNAME_SEARCH_DEBUG = 4,
            FILES_RELAY_1 = 5,
            FILE_SERVER_1 = 11,
            FILE_SERVER_2 = 12,
            FILES_RELAY_2 = 13,
            ID_SERVER_NODE_ID = 8,
            ID_SERVER_NODE_ID_DEBUG = 4,
            IX2D_DEBUG = 4,
            LOG_SERVER_1 = 8,
            LOG_SERVER_DEBUG = 4,
            MAINTENANCE_CONTROLLER = 8,
            MULTIMEDIA_SERVER_1 = 12,
            MULTIMEDIA_SERVER_DEBUG = 4,
            TRANSFER_SERVER_1 = 6,
            TRANSFER_SERVER_2 = 9,
            TRANSFER_SERVER_3 = 10,
            TRANSFER_SERVER_4 = -1,
            TRANSFER_SERVER_5 = -2,
            TRANSFER_SERVER_6 = -3,
            TRANSFER_SERVER_DEBUG = 4,
            TURN_STUN_1 = 7,
            FLAGGING_NODE_ID = 13,
            FLAGGING_NODE_ID_DEBUG = 4,
            FLAGGING_BACKUP_NODE_ID = MULTIMEDIA_SERVER_1,
            FLAGGING_BACKUP_NODE_ID_DEBUG = MULTIMEDIA_SERVER_DEBUG,
            RETROCAUSE_MODERATOR_DEVELOPMENT=4,
            RETROCAUSE_MODERATOR =12,
            RETROCAUSE_QUANTUS_DEVELOPMENT = 4,
            RETROCAUSE_QUANTUS = 13;
        public static readonly int[] ECHAT_FILESERVER_NODES = new int[] { 
            FILE_SERVER_1
        };
        public static readonly int[] ECHAT_MULTIMEDIA_SERVER_NODES = new int[] {
#if DEBUG
            MULTIMEDIA_SERVER_DEBUG
#else
            MULTIMEDIA_SERVER_1
#endif
        };
        public static readonly int[][] USERS_QUAD_TREE_NODE_IDS_AT_EACH_LEVEL = new int[][] {
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 },
            new int[]{ ECHAT_1 }
        };

        public static readonly int[][] USERS_QUAD_TREE_NODE_IDS_AT_EACH_LEVEL_DEBUG = new int[][] {
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG },
            new int[]{ ECHAT_DEBUG }
        };

        public static readonly int[] TRANSFER_SERVERS = new int[] {
            TRANSFER_SERVER_1,
            TRANSFER_SERVER_2,
            TRANSFER_SERVER_3
        };
        public static readonly int[] TRANSFER_SERVERS_DEBUG = new int[] {
            TRANSFER_SERVER_DEBUG
        };
        public static readonly int[] FILES_RELAY_WEBSOCKET_SERVERS = new int[] {
            FILES_RELAY_1/*,
            FILES_RELAY_2*/
        };
        public static readonly int[] FILES_RELAY_WEBSOCKET_SERVERS_DEBUG = new int[] {
            CLIENT_TO_CLIENT_FILES_RELAY
        };
        public static readonly int[] FILES_RELAY_FILE_SERVERS = new int[] {
            FILE_SERVER_1/*,
            FILE_SERVER_2*/
        };
        public static readonly int[] FILES_RELAY_FILE_SERVERS_DEBUG = new int[] {
            CLIENT_TO_CLIENT_FILES_RELAY
        };
        public const int AuthenticationNodeId = 3;
        public const string _Node1IP = "192.168.0.5",
            //TODO remove request both ways when accept.
            _Node2IP = "192.168.0.4",
            _Node3IP = "192.168.0.2",
            _Node5IP = "217.160.147.177",//Ionos 96659165
            _Node6IP = "82.165.204.164",//Ionos 97673121
            _Node7IP = "82.165.204.165",//Ionos 97673126
            _Node8IP = "82.165.220.32",//Ionos 98236225
            _Node9IP = "195.20.254.151",//Ionos 98448584
            _Node10IP = "82.165.6.14",//Ionos 98465028
            _Node11IP = "74.208.106.121",//Ionos 
            _Node12IP = "82.165.7.25",//Ionos 
            _Node13IP = "82.165.204.165",//Ionos 97673126
            _Node14IP = "NO IP YET";//Ionos 97673126

        public static readonly Dictionary<long, string> _MapNodeIdToIp
            = new Dictionary<long, string> {
                { 1, _Node1IP},
                { 2, _Node2IP},
                { 3, _Node3IP},
                { 5, _Node5IP},
                { 6, _Node6IP},
                { 7, _Node7IP},
                { 8, _Node8IP},
                { 9, _Node9IP},
                { 10, _Node10IP},
                { 11, _Node11IP},
                { 12, _Node12IP},
                { 13, _Node13IP}
            };
        private static readonly Dictionary<long, string[]> _MapNodeIdToDomains = new Dictionary<long, string[]>
        {
            //FIRST DOMAIN MUST BE UNIQUE TO THIS SERVER. COS IT WILL BE USED TO SPEAK TO IT THROUGH WEBSOCKETS.
                { 1, new string[]{ }},
                { 2, new string[]{ }},
                { 3, new string[]{ }},
                { 5, new string[]{ "ws.filesrelay.com", "ws.objmesh.com" }},
                { 6, new string[]{ "ts.filesrelay.com", "ts.objmesh.com" }},
                { 7, new string[]{ }},
                { 8, new string[]{  "log.objmesh.com", "maintenance.objmesh.com",
                                    "ids.objmesh.com"}},
                { 9, new string[]{ "ts2.filesrelay.com" , "ts2.objmesh.com" }},
                { 10, new string[]{ "ts3.filesrelay.com", "ts3.objmesh.com" }},
                { 11, new string[]{"fs.filesrelay.com", "fs.objmesh.com", "fs.e-chat.live",
                    "dev.e-chat.live", "e-chat.live", "www.e-chat.live", "filesrelay.com", "www.filesrelay.com"}},

                { 12, new string[]{ "ms.e-chat.live", "fs2.filesrelay.com", "fs2.objmesh.com",
                                        "filesrelay.com", "www.filesrelay.com"}},

                { 13, new string[]{ "ws.e-chat.live", "ws2.filesrelay.com", "ws2.objmesh.com" }}
        };
        private static readonly Dictionary<int, int[]> _MapNodeIdToAssociatedIdTypes = new Dictionary<int, int[]>
        {
            //FIRST DOMAIN MUST BE UNIQUE TO THIS SERVER. COS IT WILL BE USED TO SPEAK TO IT THROUGH WEBSOCKETS.
#if DEBUG
            { 1, new int[]{ }},
                { 2, new int[]{ }},
                { 3, new int[]{ }},
                { 4, new int[]{ IdTypes.CHAT_ROOM, IdTypes.CONVERSATION, IdTypes.MESSAGE, IdTypes.USER, IdTypes.MENTION}},
                { 5, new int[]{ }},
                { 6, new int[]{  }},
                { 7, new int[]{ }},
                { 8, new int[]{  }},
                { 9, new int[]{ }},
                { 10, new int[]{ }},
                { 11, new int[]{ }},

                { 12, new int[]{ }},

                { 13, new int[]{}},

                { 14, new int[]{ }}
#else
            { 1, new int[]{ }},
                { 2, new int[]{ }},
                { 3, new int[]{ }},
                { 4, new int[]{}},
                { 5, new int[]{ }},
                { 6, new int[]{  }},
                { 7, new int[]{ }},
                { 8, new int[]{  }},
                { 9, new int[]{ }},
                { 10, new int[]{ }},
                { 11, new int[]{ }},

                { 12, new int[]{ }},

                { 13, new int[]{ IdTypes.CHAT_ROOM, IdTypes.CONVERSATION, IdTypes.MESSAGE, IdTypes.USER, IdTypes.MENTION}}
#endif
        };
        private const string _MapNodeIdToIdentifierCharacter = "_0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static readonly Dictionary<char, int> _MapIdentifierCharacterToNodeId;
        static Nodes() {
            _MapIdentifierCharacterToNodeId = new Dictionary<char, int>();
            for (int i = 0; i < _MapNodeIdToIdentifierCharacter.Length; i++) {
                _MapIdentifierCharacterToNodeId[_MapNodeIdToIdentifierCharacter[i]] = i;
            }
        }
        public static int[] GetNodeIdsAssociatedWithIdType(int idType) {
            return _MapNodeIdToAssociatedIdTypes
                .Where(p => p.Value.Contains(idType))
                .Select(p => p.Key).ToArray();
        }
        public static int[] GetAssociatedIdTypes(int nodeId) {
            return _MapNodeIdToAssociatedIdTypes[nodeId];
        }
        public static char GetNodeIdentifierCharacter(int nodeId)
        {
            return _MapNodeIdToIdentifierCharacter[nodeId];
        }
        public static int GetNodeIdFromIdentifierCharacter(char c)
        {
            return _MapIdentifierCharacterToNodeId[c];
        }
        public static string[] GetDomainsForNode(int nodeId) {
            return _MapNodeIdToDomains[nodeId];
        }
        public static string FirstUniqueDomainForNode(int nodeId)
        {
            string[] domains = _MapNodeIdToDomains[nodeId];
            foreach (string domain in domains) {
                if (!_MapNodeIdToDomains.Where(p => p.Key != nodeId)
                    .Where(p => p.Value.Contains(domain)).Any())
                    return domain;
            }
            throw new Exception($"No unique domains for node {nodeId}");
        }
        public static string[] UniqueDomainsForNode(int nodeId)
        {
            string[] domains = _MapNodeIdToDomains[nodeId];
            List<string> uniqueDomains = new List<string>();
            foreach (string domain in domains) {
                if (!_MapNodeIdToDomains.Where(p => p.Key != nodeId).Where(p => p.Value.Contains(domain)).Any())
                    uniqueDomains.Add(domain);
            }
            return domains.ToArray();
        }
        public static string GetIpOrDomainForNode(int nodeId) {
            return _MapNodeIdToIp[nodeId];
        }


        public static readonly HashTagsNodeShardMapping HASH_TAGS_NODE_SHARD_MAPPINGS_DEBUG =
            new HashTagsNodeShardMapping(4, new HashTagsNodeShardMapping[]{
                new HashTagsNodeShardMapping(new char[]{'a', 'b','c','d','e','f','g','h','i','j','k','l' }, 4),
                new HashTagsNodeShardMapping('m', 4, new HashTagsNodeShardMapping[]{
                        new HashTagsNodeShardMapping(new char[] { 'a', 'b'}, 4)
                })
                //In this case anything after 'm' will default to 4 from this level
            });
        public static readonly HashTagsNodeShardMapping HASH_TAGS_NODE_SHARD_MAPPINGS =
            new HashTagsNodeShardMapping(ECHAT_1, new HashTagsNodeShardMapping[]{
                new HashTagsNodeShardMapping(new char[]{
                    '_','0','1','2','3','4','5','6','7','8','9',
                    'a','b','c','d','e','f','g','h','i','j','k',
                    'l','m','n','o','p','q','r','s','t','u','v',
                    'w','x','y', 'z',
                    'A','B','C','D','E','F','G','H','I','J','K',
                    'L','M','N','O','P','Q','R','S','T','U','V',
                    'W','X','Y','Z'
                }, ECHAT_1)
            });
    }
}