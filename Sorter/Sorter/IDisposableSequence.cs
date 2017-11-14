using System;
using System.Collections.Generic;

namespace Sorter
{
    public interface IDisposableSequence<out T> 
        : IEnumerable<T>, IDisposable
        where T : IDisposable
    {
    }
}