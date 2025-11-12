using System;

namespace MunicipalServicesApp.Structures.Trees
{
    /// <summary>
    /// Minimal AVL (insert + find) keyed by long ticks (CreatedAt).
    /// </summary>
    public class AvlNode<TKey, TValue> where TKey : IComparable<TKey>
    {
        public TKey Key;
        public TValue Value;
        public AvlNode<TKey, TValue> Left, Right;
        public int Height = 1;
        public AvlNode(TKey key, TValue value) { Key = key; Value = value; }
    }

    public class AvlTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private AvlNode<TKey, TValue> _root;

        public void Insert(TKey key, TValue value) => _root = Insert(_root, key, value);

        private static int H(AvlNode<TKey, TValue> n) => n?.Height ?? 0;
        private static int Balance(AvlNode<TKey, TValue> n) => n == null ? 0 : H(n.Left) - H(n.Right);

        private static AvlNode<TKey, TValue> Update(AvlNode<TKey, TValue> n)
        {
            n.Height = Math.Max(H(n.Left), H(n.Right)) + 1;
            return n;
        }

        private static AvlNode<TKey, TValue> RotateRight(AvlNode<TKey, TValue> y)
        {
            var x = y.Left; var T2 = x.Right;
            x.Right = y; y.Left = T2;
            Update(y); Update(x); return x;
        }

        private static AvlNode<TKey, TValue> RotateLeft(AvlNode<TKey, TValue> x)
        {
            var y = x.Right; var T2 = y.Left;
            y.Left = x; x.Right = T2;
            Update(x); Update(y); return y;
        }

        private AvlNode<TKey, TValue> Insert(AvlNode<TKey, TValue> n, TKey key, TValue value)
        {
            if (n == null) return new AvlNode<TKey, TValue>(key, value);
            int cmp = key.CompareTo(n.Key);
            if (cmp < 0) n.Left = Insert(n.Left, key, value);
            else if (cmp > 0) n.Right = Insert(n.Right, key, value);
            else { n.Value = value; return n; }

            Update(n);
            int bal = Balance(n);

            // LL
            if (bal > 1 && key.CompareTo(n.Left.Key) < 0) return RotateRight(n);
            // RR
            if (bal < -1 && key.CompareTo(n.Right.Key) > 0) return RotateLeft(n);
            // LR
            if (bal > 1 && key.CompareTo(n.Left.Key) > 0) { n.Left = RotateLeft(n.Left); return RotateRight(n); }
            // RL
            if (bal < -1 && key.CompareTo(n.Right.Key) < 0) { n.Right = RotateRight(n.Right); return RotateLeft(n); }

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
