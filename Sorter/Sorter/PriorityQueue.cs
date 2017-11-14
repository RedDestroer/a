using System;
using System.Collections.Generic;
using System.Linq;

namespace Sorter
{
    public class PriorityQueue<T>
    {
        private const int c_initialCapacity = 4;
        private readonly IComparer<T> m_comparer;
        private T[] m_items;
        private int m_count;

        public PriorityQueue()
            : this(Comparer<T>.Default)
        {
        }

        public PriorityQueue(IComparer<T> comparer)
            : this(comparer, c_initialCapacity)
        {
        }

        public PriorityQueue(IComparer<T> comparer, int capacity)
        {
            m_comparer = comparer;
            m_items = new T[capacity];
        }

        public PriorityQueue(IEnumerable<T> source)
            : this(source, Comparer<T>.Default)
        {
        }

        public PriorityQueue(IEnumerable<T> source, IComparer<T> comparer)
        {
            m_comparer = comparer;
            // In most cases queue that is created out of sequence
            // of items will be emptied step by step rather than
            // new items added and thus initially the queue is
            // not expanded but rather left full
            m_items = source.ToArray();
            m_count = m_items.Length;
            // Restore heap order
            FixWhole();
        }

        public int Capacity
        {
            get { return m_items.Length; }
        }

        public int Count
        {
            get { return m_count; }
        }

        public void Enqueue(T e)
        {
            m_items[m_count++] = e;
            // Restore heap if it was broken
            FixUp(m_count - 1);
            // Once items count reaches half of the queue capacity
            // it is doubled
            if (m_count >= m_items.Length / 2)
            {
                Expand(m_items.Length * 2);
            }
        }

        public T Dequeue()
        {
            var e = m_items[0];
            m_items[0] = m_items[--m_count];
            // Restore heap if it was broken
            FixDown(0);
            // Once items count reaches one eighth  of the queue
            // capacity it is reduced to half so that items
            // still occupy one fourth (if it is reduced when
            // count reaches one fourth after reduce items will
            // occupy half of queue capacity and next enqueued item
            // will require queue expand)
            if (m_count <= m_items.Length / 8)
            {
                Expand(m_items.Length / 2);
            }

            return e;
        }

        public T Peek()
        {
            return m_items[0];
        }

        private void FixWhole()
        {
            // Using bottom-up heap construction method enforce
            // heap property
            for (int k = m_items.Length / 2 - 1; k >= 0; k--)
            {
                FixDown(k);
            }
        }

        private void FixUp(int i)
        {
            // Make sure that starting with i-th node up to the root
            // the tree satisfies the heap property: if B is a child
            // node of A, then key(A) ≤ key(B)
            for (int c = i, p = Parent(c); c > 0; c = p, p = Parent(p))
            {
                if (Compare(m_items[p], m_items[c]) < 0)
                {
                    break;
                }
                Swap(m_items, c, p);
            }
        }

        private void FixDown(int i)
        {
            // Make sure that starting with i-th node down to the leaf
            // the tree satisfies the heap property: if B is a child
            // node of A, then key(A) ≤ key(B)
            for (int p = i, c = FirstChild(p); c < m_count; p = c, c = FirstChild(c))
            {
                if (c + 1 < m_count && Compare(m_items[c + 1], m_items[c]) < 0)
                {
                    c++;
                }
                if (Compare(m_items[p], m_items[c]) < 0)
                {
                    break;
                }
                Swap(m_items, p, c);
            }
        }

        private static int Parent(int i)
        {
            return (i - 1) / 2;
        }

        private static int FirstChild(int i)
        {
            return i * 2 + 1;
        }

        private int Compare(T a, T b)
        {
            return m_comparer.Compare(a, b);
        }

        private void Expand(int capacity)
        {
            Array.Resize(ref m_items, capacity);
        }

        private static void Swap(T[] arr, int i, int j)
        {
            var t = arr[i];
            arr[i] = arr[j];
            arr[j] = t;
        }
    }
}