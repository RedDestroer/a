using System;

namespace Sorter
{
    public class Record
    {
        private readonly int _number;
        private readonly string _str;
        private readonly Lazy<int> _hash;

        public Record(int number, string str)
        {
            _number = number;
            _str = str;
            _hash = new Lazy<int>(() => FnvHash.CreateHash(_number, _str));
        }

        public int Number
        {
            get { return _number; }
        }

        public string Str
        {
            get { return _str; }
        }

        public override bool Equals(object obj)
        {
            var other = obj as Record;
            if (other == null)
                return false;

            if (_number != other.Number)
                return false;

            return string.Equals(_str, other._str, StringComparison.InvariantCulture);
        }

        public override int GetHashCode()
        {
            return _hash.Value;
        }

        public static bool operator <(Record left, Record right)
        {
            int res = string.Compare(left._str, right._str, StringComparison.InvariantCulture);
            if (res == 0)
            {
                return left.Number < right.Number;
            }

            return res == -1;
        }

        public static bool operator >(Record left, Record right)
        {
            int res = string.Compare(left._str, right._str, StringComparison.InvariantCulture);
            if (res == 0)
            {
                return left.Number > right.Number;
            }

            return res == 1;
        }

        public static bool operator ==(Record left, Record right)
        {
            if (ReferenceEquals(left, right))
                return true;

            if (((object)left == null) || ((object)right == null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(Record left, Record right)
        {
            return !(left == right);
        }
    }
}