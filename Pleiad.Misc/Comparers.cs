using System;
using System.Collections.Generic;
using System.Linq;

namespace Pleiad.Misc
{
    namespace Comparers
    {
        public class TypeArrayComparer : IEqualityComparer<Type[]>
        {
            public bool Equals(Type[] x, Type[] y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (x == null || y == null)
                    return false;
                return x.SequenceEqual(y);
            }

            public int GetHashCode(Type[] obj)
            {
                return obj.Aggregate(42, (res, item) => unchecked(res * 16 + item.GetHashCode()));
            }
        }
    }
}
