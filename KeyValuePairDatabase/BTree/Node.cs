namespace BTree
{
    using Constants;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class Node<TKey, TPointer> where TKey : IComparable<TKey>
    {
        private int _Degree;

        public Node(int degree)
        {
            _Degree = degree;
            Children = new List<Node<TKey, TPointer>>(degree);
            Entries = new List<Entry<TKey, TPointer>>(degree);
        }

        public List<Node<TKey, TPointer>> Children { get; protected set; }

        public List<Entry<TKey, TPointer>> Entries { get; protected set; }

        public bool IsLeaf
        {
            get
            {
                return Children.Count == 0;
            }
        }

        public bool HasReachedMaxEntries
        {
            get
            {
                return Entries.Count >= (2 * _Degree) - 1;
            }
        }

        public bool HasReachedMinEntries
        {
            get
            {
                return Entries.Count <= _Degree - 1;
            }
        }
        public void InsertNonFull(TKey newKey, TPointer newPointer)
        {
            int positionToInsert = Entries.TakeWhile(entry => newKey
                .CompareTo(entry.Key) >= 0).Count();

            // leaf node
            if (IsLeaf)
            {
                Entries.Insert(positionToInsert, new Entry<TKey, TPointer>() { Key = newKey, Pointer = newPointer });
                return;
            }

            // non-leaf
            Node<TKey, TPointer> child = Children[positionToInsert];
            if (child.HasReachedMaxEntries)
            {
                SplitChild(positionToInsert, child);
                if (newKey.CompareTo(Entries[positionToInsert].Key) > 0)
                {
                    positionToInsert++;
                }
            }
            Children[positionToInsert].InsertNonFull(newKey, newPointer);
        }
        public void SplitChild(int nodeToBeSplitIndex, Node<TKey, TPointer> nodeToBeSplit)
        {
            var newNode = new Node<TKey, TPointer>(_Degree);

            Entries.Insert(nodeToBeSplitIndex, nodeToBeSplit.Entries[_Degree - 1]);
            Children.Insert(nodeToBeSplitIndex + 1, newNode);

            newNode.Entries.AddRange(nodeToBeSplit.Entries.GetRange(_Degree, _Degree - 1));

            // remove also Entries[this.Degree - 1], which is the one to move up to the parent
            nodeToBeSplit.Entries.RemoveRange(_Degree - 1, _Degree);

            if (nodeToBeSplit.IsLeaf)
            {
                return;
            }
            newNode.Children.AddRange(nodeToBeSplit.Children.GetRange(_Degree, _Degree));
            nodeToBeSplit.Children.RemoveRange(_Degree, _Degree);
        }
        #region Search
        public Entry<TKey, TPointer> Search(TKey key)
        {
            int? index = FindIndex(key);

            if (index!=null)
            {
                return Entries[(int)index];
            }
            return IsLeaf ? null : Children[Entries.Count].Search(key);
        }
        #endregion Search
        #region Deletion
        public void DeleteKeyFromSubtree(TKey keyToDelete, int subtreeIndexInNode)
        {
            Node<TKey, TPointer> childNode = Children[subtreeIndexInNode];
            if (childNode.HasReachedMinEntries)
            {
                childNode.Delete(keyToDelete);
                return;
            }
            int leftIndex = subtreeIndexInNode - 1;
            Node<TKey, TPointer> leftSibling = subtreeIndexInNode > 0 ? Children[leftIndex] : null;


            if (leftSibling != null && leftSibling.Entries.Count > _Degree - 1)
            {
                // left sibling has a node to spare, so this moves one node from left sibling 
                // into parent's node and one node from parent into this current node ("child")
                childNode.Entries.Insert(0, Entries[subtreeIndexInNode]);
                Entries[subtreeIndexInNode] = leftSibling.Entries.Last();
                leftSibling.Entries.RemoveAt(leftSibling.Entries.Count - 1);

                if (!leftSibling.IsLeaf)
                {
                    childNode.Children.Insert(0, leftSibling.Children.Last());
                    leftSibling.Children.RemoveAt(leftSibling.Children.Count - 1);
                }
                childNode.Delete(keyToDelete);
                return;
            }

            int rightIndex = subtreeIndexInNode + 1;
            Node<TKey, TPointer> rightSibling = subtreeIndexInNode < Children.Count - 1
                                            ? Children[rightIndex]
                                            : null;
            if (rightSibling != null && rightSibling.Entries.Count > _Degree - 1)
            {
                // right sibling has a node to spare, so this moves one node from right sibling 
                // into parent's node and one node from parent into this current node ("child")
                childNode.Entries.Add(Entries[subtreeIndexInNode]);
                Entries[subtreeIndexInNode] = rightSibling.Entries.First();
                rightSibling.Entries.RemoveAt(0);

                if (!rightSibling.IsLeaf)
                {
                    childNode.Children.Add(rightSibling.Children.First());
                    rightSibling.Children.RemoveAt(0);
                }
                childNode.Delete(keyToDelete);
                return;
            }
            // this block merges either left or right sibling into the current node "child"
            if (leftSibling != null)
            {
                childNode.Entries.Insert(0, Entries[subtreeIndexInNode]);
                var oldEntries = childNode.Entries;
                childNode.Entries = leftSibling.Entries;
                childNode.Entries.AddRange(oldEntries);
                if (!leftSibling.IsLeaf)
                {
                    var oldChildren = childNode.Children;
                    childNode.Children = leftSibling.Children;
                    childNode.Children.AddRange(oldChildren);
                }

                Children.RemoveAt(leftIndex);
                Entries.RemoveAt(subtreeIndexInNode);
                childNode.Delete(keyToDelete);
                return;
            }
            Debug.Assert(rightSibling != null, "Node should have at least one sibling");
            childNode.Entries.Add(Entries[subtreeIndexInNode]);
            childNode.Entries.AddRange(rightSibling.Entries);
            if (!rightSibling.IsLeaf)
            {
                childNode.Children.AddRange(rightSibling.Children);
            }
            Children.RemoveAt(rightIndex);
            Entries.RemoveAt(subtreeIndexInNode);
            childNode.Delete(keyToDelete);
        }
        
        private void DeleteKeyFromNode(TKey keyToDelete, int keyIndexInNode)
        {
            // if leaf, just remove it from the list of entries (we're guaranteed to have
            // at least "degree" # of entries, to BTree property is maintained
            if (IsLeaf)
            {
                Entries.RemoveAt(keyIndexInNode);
                return;
            }
            Node<TKey, TPointer> predecessorChild = Children[keyIndexInNode];
            if (predecessorChild.Entries.Count >= _Degree)
            {
                Entry<TKey, TPointer> predecessor = predecessorChild.DeletePredecessor();
                Entries[keyIndexInNode] = predecessor;
            }
            else
            {
                Node<TKey, TPointer> successorChild = Children[keyIndexInNode + 1];
                if (successorChild.Entries.Count >= _Degree)
                {
                    Entry<TKey, TPointer> successor = predecessorChild.DeleteSuccessor();
                    Entries[keyIndexInNode] = successor;
                }
                else
                {
                    predecessorChild.Entries.Add(Entries[keyIndexInNode]);
                    predecessorChild.Entries.AddRange(successorChild.Entries);
                    predecessorChild.Children.AddRange(successorChild.Children);

                    Entries.RemoveAt(keyIndexInNode);
                    Children.RemoveAt(keyIndexInNode + 1);

                    predecessorChild.Delete(keyToDelete);
                }
            }
        }
        private Entry<TKey, TPointer> DeletePredecessor()
        {
            if (IsLeaf)
            {
                var result = Entries[Entries.Count - 1];
                Entries.RemoveAt(Entries.Count - 1);
                return result;
            }

            return Children.Last().DeletePredecessor();
        }

        /// <summary>
        /// Helper method that deletes a successor key (i.e. leftmost key) for a given node.
        /// </summary>
        /// <param name="node">Node for which the successor will be deleted.</param>
        /// <returns>Successor entry that got deleted.</returns>
        private Entry<TKey, TPointer> DeleteSuccessor()
        {
            if (IsLeaf)
            {
                var result = Entries[0];
                Entries.RemoveAt(0);
                return result;
            }

            return Children.First().DeletePredecessor();
        }
        public void Delete(TKey keyToDelete)
        {

            // found key in node, so delete if from it
            int? index = FindIndex(keyToDelete);
            if (index!=null)
            {
                DeleteKeyFromNode(keyToDelete, (int)index);
                return;
            }
            if (IsLeaf)
            {
                DeleteKeyFromSubtree(keyToDelete, Entries.Count);
            }
        }
        #endregion Deletion
        private int? FindIndex(TKey keyToDelete) {
            for (int i = 0; i < Entries.Count; i++)
            {
                if (Entries[i].Key.Equals(keyToDelete))
                    return i;
            }
            return null;
        }
    }
}
