using System.Collections.Generic;

namespace Sorter
{
    public class RecordComparer
        : IComparer<Record>
    {
        public int Compare(Record x, Record y)
        {
            return x.CompareTo(y);

            ////if (x < y)
            ////    return -1;

            ////if (x > y)
            ////    return 1;

            ////return 0;
        }
    }
}