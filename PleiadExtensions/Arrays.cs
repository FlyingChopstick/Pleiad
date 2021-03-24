using System.Collections.Generic;
using System.Linq;

namespace PleiadExtensions
{
    namespace Arrays
    {
        /// <summary>
        /// Extensions for arrays
        /// </summary>
        public static class ArrayExtensions
        {
            /// <summary>
            /// Tests if the array is a subset of another array
            /// </summary>
            /// <typeparam name="T">Array type</typeparam>
            /// <param name="subset">Target</param>
            /// <param name="source">Array to test against</param>
            /// <returns><see langword="true"/> if is a subset, <see langword="false"/> otherwise</returns>
            public static bool IsSubsetOf<T>(this T[] subset, IEnumerable<T> source)
            {
                return subset.Length < source.Count() && subset.Where(t => source.Contains(t)).Count() == subset.Length;
            }

            /// <summary>
            /// Tests if the array is a subset of another array
            /// </summary>
            /// <typeparam name="T">Array type</typeparam>
            /// <param name="subset">Target</param>
            /// <param name="source">Array to test against</param>
            /// <returns><see langword="true"/> if is a subset, <see langword="false"/> otherwise</returns>
            public static bool IsSubsetOf<T>(this T[] subset, T[] source)
            {
                return subset.Length < source.Length && subset.Where(t => source.Contains(t)).Count() == subset.Length;
            }
        }
    }
}
