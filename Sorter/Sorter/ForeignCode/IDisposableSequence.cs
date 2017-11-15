using System;
using System.Collections.Generic;

namespace Sorter.ForeignCode
{
    /// <summary>
    /// Вспомогательный интерфейс, позволяющий красиво обернуть последовательность вида IEnumerable
    /// где все элементы удовлетворяют интерфейсу IDisposable
    /// в using, чтобы по окончании обработки всех элементов последовательности у них автоматически
    /// были вызваны их методы Dispose()
    /// 
    /// Код взят из этого блога
    /// https://blogs.msdn.microsoft.com/dhuba/2010/08/24/external-merge-sort/
    /// синтаксис автора менять не стал
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDisposableSequence<out T> 
        : IEnumerable<T>, IDisposable
        where T : IDisposable
    {
    }
}