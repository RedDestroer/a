using System.Collections.Generic;

namespace Sorter
{
    public class OrdinalRecordComparer
        : IComparer<OrdinalRecord>
    {
        public int Compare(OrdinalRecord x, OrdinalRecord y)
        {
            return x.CompareTo(y);
        }
    }
}