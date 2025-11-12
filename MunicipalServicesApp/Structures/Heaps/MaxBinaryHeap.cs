using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Structures.Heaps
{
    /// <summary>
    /// Max-heap based on IComparer<T>. Used to pop highest priority requests.
    /// </summary>
    public class MaxBinaryHeap<T>
    {
        private readonly List<T> _a = new List<T>();
        private readonly IComparer<T> _cmp;
        public MaxBinaryHeap(IComparer<T> comparer) { _cmp = comparer ?? Comparer<T>.Default; }

        public int Count => _a.Count;
        public void Clear() => _a.Clear();

        public void Push(T v)
        {
            _a.Add(v);
            SiftUp(_a.Count - 1);
        }

        public bool TryPop(out T v)
        {
            if (_a.Count == 0) { v = default(T); return false; }
            v = _a[0];
            var last = _a[_a.Count - 1];
            _a.RemoveAt(_a.Count - 1);
            if (_a.Count > 0) { _a[0] = last; SiftDown(0); }
            return true;
        }

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int p = (i - 1) / 2;
                if (_cmp.Compare(_a[i], _a[p]) <= 0) break;
                (_a[i], _a[p]) = (_a[p], _a[i]);
                i = p;
            }
        }

        private void SiftDown(int i)
        {
            int n = _a.Count;
            while (true)
            {
                int l = 2 * i + 1, r = l + 1, m = i;
                if (l < n && _cmp.Compare(_a[l], _a[m]) > 0) m = l;
                if (r < n && _cmp.Compare(_a[r], _a[m]) > 0) m = r;
                if (m == i) break;
                (_a[i], _a[m]) = (_a[m], _a[i]);
                i = m;
            }
        }
    }
}
