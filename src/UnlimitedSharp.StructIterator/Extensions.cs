using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace UnlimitedSharp
{
    internal static class Extensions
    {
        public static void InPlaceAdd<T>(ref this ImmutableArray<T> array, T element) => array = array.Add(element);

        public static V FirstValue<K, V>(this Dictionary<K, V> dict)
        {
            foreach (var value in dict.Values)
                return value;

            return default!;
        }

        public static void AddMemberToMultiValueDictionary(this Dictionary<ReadOnlyMemory<char>, object> accumulator, Symbol member) => Microsoft.CodeAnalysis.
            ImmutableArrayExtensions.AddToMultiValueDictionaryBuilder(accumulator, member.Name.AsMemory(), member);

        extension(SourceOrdinaryMethodSymbol m)
        {
            public MethodDeclarationSyntax Syntax => m.GetSyntax();
        }
    }
}