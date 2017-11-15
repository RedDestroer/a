using System;
using System.Collections.Generic;

namespace Sorter.ForeignCode
{
    /// <summary>
    /// Код взят из этого блога
    /// https://blogs.msdn.microsoft.com/dhuba/2010/08/24/external-merge-sort/
    /// синтаксис автора менять не стал
    /// </summary>
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