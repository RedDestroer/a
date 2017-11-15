using System;

namespace Sorter
{
    public class OrdinalRecord
        : IComparable, IComparable<OrdinalRecord>
    {
        private readonly int _number;
        private readonly char[] _str;

        public OrdinalRecord(int number, char[] str)
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
            var other = obj as OrdinalRecord;
            if (other == null)
                return 1;

            return CompareTo(other);
        }

        public int CompareTo(OrdinalRecord other)
        {
            if (ReferenceEquals(this, other))
                return 0;
            if (ReferenceEquals(null, other))
                return 1;

            bool? isLess = IsLess(_str, other._str);
            if (isLess != null)
            {
                return isLess.Value
                    ? -1
                    : 1;
            }

            return _number.CompareTo(other._number);
        }

        private static bool? IsLess(char[] left, char[] right)
        {
            int minStrLength = Math.Min(left.Length, right.Length);
            for (int i = 0; i < minStrLength; i++)
            {
                int charCompare = left[i].CompareTo(right[i]);
                if (charCompare < 0)
                    return true;
                if (charCompare > 0)
                    return false;
            }

            if (left.Length < right.Length)
                return true;

            if (left.Length > right.Length)
                return false;

            return null;
        }
    }
}