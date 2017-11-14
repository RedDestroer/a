using System.Collections;
using System.Linq;

namespace Sorter
{
    public static class FnvHash
    {
        public static readonly int OffsetBasis = unchecked((int)2166136261);
        public static readonly int Prime = 16777619;

        public static int CreateHash(params object[] objs)
        {
            return objs.Aggregate(
                OffsetBasis,
                (r, o) =>
                {
                    if (o == null)
                        return r * Prime;

                    var str = o as string;
                    if (str != null)
                        return (r ^ str.GetHashCode()) * Prime;

                    var enumerable = o as IEnumerable;
                    if (enumerable != null)
                        return (r ^ CreateHash(enumerable)) * Prime;

                    return (r ^ o.GetHashCode()) * Prime;
                });
        }

        public static int CreateHash(IEnumerable enumerable)
        {
            return enumerable.OfType<object>()
                .Aggregate(
                    OffsetBasis, (r, o) =>
                    {
                        return (r ^ o.GetHashCode()) * Prime;
                    });
        }
    }
}