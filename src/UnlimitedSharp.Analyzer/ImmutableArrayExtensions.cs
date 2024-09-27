using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace UnlimitedSharp
{
    static class ImmutableArrayExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InPlaceInsert<T>(ref this ImmutableArray<T> self, int index, T item) 
            => self = self.Insert(index, item);
    }
}