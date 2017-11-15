using System;
using System.Collections.Generic;
using Sorter.ForeignCode2;

namespace Sorter
{
    public class FibonacciPriorityQueue<T>
        : IQueue<T>
        where T : IComparable
    {
        private readonly int _capacity;
        private readonly FibonacciHeap<T, T> _heap;

        public FibonacciPriorityQueue(int capacity, T minKey)
        {
            _capacity = capacity;
            _heap = new FibonacciHeap<T, T>(minKey);
        }

        public int Capacity
        {
            get { return _capacity; }
        }

        public int Count
        {
            get { return _heap.Size(); }
        }

        public void Enqueue(T e)
        {
            _heap.Insert(new FibonacciHeapNode<T, T>(e, e));

            ////m_items[m_count++] = e;
            ////// Restore heap if it was broken
            ////FixUp(m_count - 1);
            ////// Once items count reaches half of the queue capacity
            ////// it is doubled
            ////if (m_count >= m_items.Length / 2)
            ////{
            ////    Expand(m_items.Length * 2);
            ////}
        }

        public T Dequeue()
        {
            var node = _heap.Min();
            _heap.Delete(node);

            ////var e = m_items[0];
            ////m_items[0] = m_items[--m_count];
            ////// Restore heap if it was broken
            ////FixDown(0);
            ////// Once items count reaches one eighth  of the queue
            ////// capacity it is reduced to half so that items
            ////// still occupy one fourth (if it is reduced when
            ////// count reaches one fourth after reduce items will
            ////// occupy half of queue capacity and next enqueued item
            ////// will require queue expand)
            ////if (m_count <= m_items.Length / 8)
            ////{
            ////    Expand(m_items.Length / 2);
            ////}

            return node.Data;
        }

        public T Peek()
        {
            var node = _heap.Min();

            return node.Data;
        }
    }
}