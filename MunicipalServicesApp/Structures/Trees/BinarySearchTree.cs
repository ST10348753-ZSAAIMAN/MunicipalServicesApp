using System;

namespace MunicipalServicesApp.Structures.Trees
{
    /// <summary>
    /// Simple (unbalanced) BST for TicketNumber -> ServiceRequest lookups.
    /// Only Insert and Find used in app.
    /// </summary>
    public class BstNode<TKey, TValue> where TKey : IComparable<TKey>
    {
        public TKey Key;
        public TValue Value;
        public BstNode<TKey, TValue> Left, Right;
        public BstNode(TKey key, TValue value) { Key = key; Value = value; }
    }

    public class BinarySearchTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private BstNode<TKey, TValue> _root;

        public void Insert(TKey key, TValue value) => _root = Insert(_root, key, value);

        private BstNode<TKey, TValue> Insert(BstNode<TKey, TValue> n, TKey key, TValue value)
        {
            if (n == null) return new BstNode<TKey, TValue>(key, value);
            int cmp = key.CompareTo(n.Key);
            if (cmp < 0) n.Left = Insert(n.Left, key, value);
            else if (cmp > 0) n.Right = Insert(n.Right, key, value);
            else n.Value = value;
            return n;
        }

        public bool TryFind(TKey key, out TValue value)
        {
            var n = _root;
            while (n != null)
            {
                int cmp = key.CompareTo(n.Key);
                if (cmp == 0) { value = n.Value; return true; }
                n = cmp < 0 ? n.Left : n.Right;
            }
            value = default(TValue);
            return false;
        }
    }
}
