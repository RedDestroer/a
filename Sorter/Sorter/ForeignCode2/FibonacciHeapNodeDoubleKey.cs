namespace Sorter.ForeignCode2
{
    /// <summary>
    /// Implementation of the Fibonacci heap node with key of type double.
    /// </summary>
    /// <typeparam name="T">Type of the data to be stored.</typeparam>
    public class FibonacciHeapNodeDoubleKey<T> : FibonacciHeapNode<T, double>
    {
        public FibonacciHeapNodeDoubleKey(T data, double key)
            : base(data, key)
        {
        }
    }

    public class FibonacciHeapNodeRecordKey 
        : FibonacciHeapNode<Record, Record>
    {
        public FibonacciHeapNodeRecordKey(Record data)
            : base(data, data)
        {
        }
    }
}