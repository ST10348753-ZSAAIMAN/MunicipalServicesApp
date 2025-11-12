using System;

namespace MunicipalServicesApp.Structures.Trees
{
    /// <summary>
    /// Minimal Red-Black Tree (insert + find). Left-leaning style.
    /// Colors: RED=true, BLACK=false.
    /// </summary>
    public class RbNode<TKey, TValue> where TKey : IComparable<TKey>
    {
        public TKey Key; public TValue Value;
        public RbNode<TKey, TValue> Left, Right;
        public bool Red; // true=RED, false=BLACK
        public RbNode(TKey key, TValue value, bool red) { Key = key; Value = value; Red = red; }
    }

    public class RedBlackTree<TKey, TValue> where TKey : IComparable<TKey>
    {
        private RbNode<TKey, TValue> _root;

        private static bool IsRed(RbNode<TKey, TValue> x) => x != null && x.Red;

        private static RbNode<TKey, TValue> RotateLeft(RbNode<TKey, TValue> h)
        {
            var x = h.Right;
            h.Right = x.Left; x.Left = h; x.Red = h.Red; h.Red = true; return x;
        }
        private static RbNode<TKey, TValue> RotateRight(RbNode<TKey, TValue> h)
        {
            var x = h.Left;
            h.Left = x.Right; x.Right = h; x.Red = h.Red; h.Red = true; return x;
        }
        private static void FlipColors(RbNode<TKey, TValue> h)
        {
            h.Red = true; if (h.Left != null) h.Left.Red = false; if (h.Right != null) h.Right.Red = false;
        }

        public void Insert(TKey key, TValue value)
        {
            _root = Insert(_root, key, value);
            _root.Red = false;
        }

        private RbNode<TKey, TValue> Insert(RbNode<TKey, TValue> h, TKey key, TValue value)
        {
            if (h == null) return new RbNode<TKey, TValue>(key, value, true);

            int cmp = key.CompareTo(h.Key);
            if (cmp < 0) h.Left = Insert(h.Left, key, value);
            else if (cmp > 0) h.Right = Insert(h.Right, key, value);
            else h.Value = value;

            // fix-ups
            if (IsRed(h.Right) && !IsRed(h.Left)) h = RotateLeft(h);
            if (IsRed(h.Left) && IsRed(h.Left.Left)) h = RotateRight(h);
            if (IsRed(h.Left) && IsRed(h.Right)) FlipColors(h);

            return h;
        }

        public bool TryFind(TKey key, out TValue value)
        {
            var x = _root;
            while (x != null)
            {
                int cmp = key.CompareTo(x.Key);
                if (cmp == 0) { value = x.Value; return true; }
                x = cmp < 0 ? x.Left : x.Right;
            }
            value = default(TValue);
            return false;
        }
    }
}
