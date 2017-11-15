using System;
using System.Collections;
using System.Collections.Generic;

namespace Sorter.ForeignCode
{
    /// <summary>
    /// Реализация вспомогательного интерфейс, позволяющего красиво обернуть последовательность вида IEnumerable
    /// где все элементы удовлетворяют интерфейсу IDisposable
    /// в using, чтобы по окончании обработки всех элементов последовательности у них автоматически
    /// были вызваны их методы Dispose()
    /// 
    /// Код взят из этого блога
    /// https://blogs.msdn.microsoft.com/dhuba/2010/08/24/external-merge-sort/
    /// синтаксис автора менять не стал
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisposableSequence<T> 
        : IDisposableSequence<T>
        where T : IDisposable
    {
        private IEnumerable<T> m_seq;
        private IEnumerator<T> m_enum;
        private Node<T> m_head;
        private bool m_disposed;

        public DisposableSequence(IEnumerable<T> sequence)
        {
            m_seq = sequence;
        }

        public IEnumerator<T> GetEnumerator()
        {
            ThrowIfDisposed();

            // Enumerator is built traversing lazy linked list
            // and forcing it to expand if possible
            var n = EnsureHead();
            while (n != null)
            {
                yield return n.Value;
                n = n.GetNext(true);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Dispose()
        {
            if (!m_disposed)
            {
                m_disposed = true;

                // As sequence creates enumerator it is responsible
                // for its disposal
                if (m_enum != null)
                {
                    m_enum.Dispose();
                    m_enum = null;
                }

                // As it is possible that not all resources were
                // obtained (for example, inside using statement
                // only half of lazy evaluated sequence elements
                // were enumerated and thus only half of resources
                // obtained) we do not want to obtain them now
                // as they are going to be disposed immediately.
                // Thus we traverse only through already created
                // lazy linked list nodes and dispose obtained
                // resources
                Dispose(m_head);

                m_seq = null;
            }
        }

        private Node<T> EnsureHead()
        {
            // Obtain enumerator once
            if (m_enum == null)
            {
                m_enum = m_seq.GetEnumerator();
                // Try to expand to first element
                if (m_enum.MoveNext())
                {
                    // Created node caches current element
                    m_head = new Node<T>(m_enum);
                }
            }
            return m_head;
        }

        private void ThrowIfDisposed()
        {
            if (m_disposed)
            {
                throw new ObjectDisposedException("DisposableSequence");
            }
        }

        private static void Dispose(Node<T> h)
        {
            if (h == null)
            {
                return;
            }

            try
            {
                // Disposing resources must be done in the opposite
                // to usage order. With recursion it will have the
                // same semantics as nested try{}finally{} blocks.
                Dispose(h.GetNext(false));
            }
            finally
            {
                h.Value.Dispose();
            }
        }

        private class Node<TV>
        {
            private readonly TV m_value;
            private IEnumerator<TV> m_enum;
            private Node<TV> m_next;

            public Node(IEnumerator<TV> enumerator)
            {
                m_value = enumerator.Current;
                m_enum = enumerator;
            }

            public TV Value
            {
                get { return m_value; }
            }

            public Node<TV> GetNext(bool force)
            {
                // Expand only if forced and not expanded before
                if (force && m_enum != null)
                {
                    if (m_enum.MoveNext())
                    {
                        m_next = new Node<TV>(m_enum);
                    }
                    m_enum = null;
                }
                return m_next;
            }
        }
    }
}