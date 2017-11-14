using System;
using System.Collections.Generic;

namespace Sorter
{
    public static class DisposableExtension
    {
        // Defined as an extension method that augments minimal needed interface
        public static IDisposableSequence<T> AsDisposable<T>(this IEnumerable<T> seq)
            where T:IDisposable
        {
            return new DisposableSequence<T>(seq);
        }
    }
}