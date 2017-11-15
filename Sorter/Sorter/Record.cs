using System;

namespace Sorter
{
    public class Record
        : IComparable, IComparable<Record>
    {
        public static readonly Record Min = new Record(0, string.Empty);

        private readonly int _number;
        private readonly string _str;

        public Record(int number, string str)
        {
            _number = number;
            _str = str;
        }
        
        public string GetText()
        {
            return _number + ". " + _str;
        }

        public int CompareTo(object obj)
        {
            var other = obj as Record;
            if (other == null)
                return 1;

            return CompareTo(other);
        }

        public int CompareTo(Record other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;

            int res = string.Compare(_str, other._str, StringComparison.InvariantCulture);
            if (res == 0)
            {
                return _number.CompareTo(other._number);
            }

            return res;
        }

        ////public static bool operator <(Record left, Record right)
        ////{
        ////    int res = string.Compare(left._str, right._str, StringComparison.InvariantCulture);
        ////    if (res == 0)
        ////    {
        ////        return left.Number < right.Number;
        ////    }

        ////    return res == -1;
        ////}

        ////public static bool operator >(Record left, Record right)
        ////{
        ////    int res = string.Compare(left._str, right._str, StringComparison.InvariantCulture);
        ////    if (res == 0)
        ////    {
        ////        return left.Number > right.Number;
        ////    }

        ////    return res == 1;
        ////}
    }
}