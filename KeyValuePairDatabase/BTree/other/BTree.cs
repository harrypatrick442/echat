using JSON;
using Core.Exceptions;
using Core.Ticketing;
using Core.Ids;
using Logging;
using InterserverComs;
using Nodes;
using System.Drawing.Printing;

namespace KeyValuePairDatabases
{
    public class BTree    
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="M">Number of child nodes</param>
        public BTree(uint keyNBytes, int valueNBytes, byte M) {
            /*
             * Internal Node
             * [from pointer 8 bytes ]
             * [is leaf 1 byte]
             * [nKeys 4 bytes] Can calculate start of values from this
             * [k1 (keyNBytes length)]
             * [k2 (keyNBytes length)]
             * [k3 (keyNBytes length)]
             * [v1 pointer 8 bytes]
             * [v2 pointer 8 bytes]
             * [v3 pointer 8 bytes]
             * [v4 pointer 8 bytes]
             * */

            /*
             * Leaf Node
             * [from internal node pointer 8 bytes ]
             * [from leaf pointer 8 bytes ]
             * [to leaf pointer 8 bytes ]
             * [value valueNBytes]
             * */
            /* problem with too high a tree is 
             * has to read more disk clusters and
             * doesnt utilize one read properly cos only tiny part of cluster
             * VERY GOOD https://benjamincongdon.me/blog/2021/08/17/B-Trees-More-Than-I-Thought-Id-Want-to-Know/
             *https://stackoverflow.com/questions/122362/how-to-empty-flush-windows-read-disk-cache-in-c
             *Alignment on disk
             * 
            /*https://workat.tech/core-cs/tutorial/b-tree-and-b-plus-trees-in-dbms-gzzjplg15e4q
             * https://en.wikipedia.org/wiki/B-tree
             * */
        }
    }
}