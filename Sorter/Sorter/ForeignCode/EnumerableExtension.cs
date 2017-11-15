using System.Collections.Generic;
using System.Linq;

namespace Sorter.ForeignCode
{
    /// <summary>
    /// Несколько методов расширения к IEnumerable
    /// 
    /// Код взят из этого блога
    /// https://blogs.msdn.microsoft.com/dhuba/2010/08/24/external-merge-sort/
    /// синтаксис автора менять не стал
    /// </summary>
    public static class EnumerableExtension
    {
        public static IEnumerable<T> OrderedMerge<T>(this IEnumerable<T> first, IEnumerable<T> second, IComparer<T> comparer)
        {
            using (var e1 = first.GetEnumerator())
            {
                using (var e2 = second.GetEnumerator())
                {
                    var c1 = e1.MoveNext();
                    var c2 = e2.MoveNext();
                    while (c1 && c2)
                    {
                        if (comparer.Compare(e1.Current, e2.Current) < 0)
                        {
                            yield return e1.Current;
                            c1 = e1.MoveNext();
                        }
                        else
                        {
                            yield return e2.Current;
                            c2 = e2.MoveNext();
                        }
                    }
                    if (c1 || c2)
                    {
                        var e = c1
                            ? e1
                            : e2;
                        do
                        {
                            yield return e.Current;
                        } while (e.MoveNext());
                    }
                }
            }
        }

        // Convenience overloads are not included only most general one
        public static IEnumerable<T> OrderedMerge<T>(this IEnumerable<IEnumerable<T>> sources, IComparer<T> comparer)
        {  
            // Precondition checking is done outside of iterator because
            // of its lazy nature
            return OrderedMergeHelper(sources, comparer);
        }

        private static IEnumerable<T> OrderedMergeHelper<T>(IEnumerable<IEnumerable<T>> sources, IComparer<T> elementComparer)
        {
            // Each sequence is expected to be ordered according to
            // the same comparison logic as elementComparer provides
            var enumerators = sources.Select(e => e.GetEnumerator());
            // Disposing sequence of lazily acquired resources as
            // a single resource
            using (var disposableEnumerators = enumerators.AsDisposable())
            {
                // The code below holds the following loop invariant:
                // - Priority queue contains enumerators that positioned at
                // sequence element
                // - The queue at the top has enumerator that positioned at
                // the smallest element of the remaining elements of all
                // sequences

                // Ensures that only non empty sequences participate  in merge
                var nonEmpty = disposableEnumerators.Where(e => e.MoveNext());
                // Current value of enumerator is its priority
                var comparer = new EnumeratorComparer<T>(elementComparer);
                // Use priority queue to get enumerator with smallest
                // priority (current value)
                var queue = new PriorityQueue<IEnumerator<T>>(nonEmpty, comparer);

                // The queue is empty when all sequences are empty
                while (queue.Count > 0)
                {
                    // Dequeue enumerator that positioned at element that
                    // is next in the merged sequence
                    var min = queue.Dequeue();
                    yield return min.Current;
                    // Advance enumerator to next value
                    if (min.MoveNext())
                    {
                        // If it has value that can be merged into resulting
                        // sequence put it into the queue
                        queue.Enqueue(min);
                    }
                }
            }
        }

        // Provides comparison functionality for enumerators
        private class EnumeratorComparer<T> 
            : Comparer<IEnumerator<T>>
        {
            private readonly IComparer<T> m_comparer;

            public EnumeratorComparer(IComparer<T> comparer)
            {
                m_comparer = comparer;
            }

            public override int Compare(
                IEnumerator<T> x, IEnumerator<T> y)
            {
                return m_comparer.Compare(x.Current, y.Current);
            }
        }
    }
}