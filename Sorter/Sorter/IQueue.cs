namespace Sorter
{
    public interface IQueue<T>
    {
        int Capacity { get; }
        int Count { get; }
        void Enqueue(T e);
        T Dequeue();
        T Peek();
    }
}