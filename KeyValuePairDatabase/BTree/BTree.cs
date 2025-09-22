namespace BTree
{
    using System;
    using System.Linq;

    /// <summary>
    /// Based on BTree chapter in "Introduction to Algorithms", by Thomas Cormen, Charles Leiserson, Ronald Rivest.
    /// 
    /// This implementation is not thread-safe, and user must handle thread-safety.
    /// </summary>
    /// <typeparam name="TKey">Type of BTree Key.</typeparam>
    /// <typeparam name="TPointer">Type of BTree Pointer associated with each Key.</typeparam>
    public class BTree<TKey, TPointer> where TKey : IComparable<TKey>
    {
        public BTree(int minimumDegree)
        {
            if (minimumDegree < 2)
            {
                throw new ArgumentException("BTree degree must be at least 2", "degree");
            }
            Root = new Node<TKey, TPointer>(minimumDegree);
            Degree = minimumDegree;
            Height = 1;
        }

        public Node<TKey, TPointer> Root { get; private set; }

        public int Degree { get; private set; }

        public int Height { get; private set; }
        public Entry<TKey, TPointer> Search(TKey key)
        {
            return Root.Search(key);
        }
        public void Insert(TKey newKey, TPointer newPointer)
        {
            if (!Root.HasReachedMaxEntries)
            {
                Root.InsertNonFull(newKey, newPointer);
                return;
            }
            Node<TKey, TPointer> oldRoot = Root;
            Root = new Node<TKey, TPointer>(Degree);
            Root.Children.Add(oldRoot);
            Root.SplitChild(0, oldRoot);
            Root.InsertNonFull(newKey, newPointer);
            Height++;
        }
        public void Delete(TKey keyToDelete)
        {
            Root.Delete(keyToDelete);

            // if root's last entry was moved to a child node, remove it
            if (Root.Entries.Count == 0 && !Root.IsLeaf)
            {
                Root = Root.Children.Single();
                Height--;
            }
        }
    }
}
