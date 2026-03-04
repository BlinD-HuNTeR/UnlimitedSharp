using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using static Microsoft.CodeAnalysis.CSharp.Symbols.SourceOrdinaryMethodSymbol;

namespace UnlimitedSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Hooks : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [];
        public override void Initialize(AnalysisContext context) { }

        static Hooks()
        {
            Unsafe.AsRef(in NamespaceOrTypeSymbol.s_nameToObjectPool) = CreatePool();

            Unsafe.AsRef(in DebugInfoInjector.s_singleton) = new(
                Unsafe.AsRef(in Instrumenter.NoOp) = new StructIteratorInstrumenter(Instrumenter.NoOp));
        }

        private static ObjectPool<PooledDictionary<ReadOnlyMemory<char>, object>> CreatePool()
        {
            ObjectPool<PooledDictionary<ReadOnlyMemory<char>, object>> pool = null!;
            pool = new ObjectPool<PooledDictionary<ReadOnlyMemory<char>, object>>(() => new HookedComparer(pool).Dictionary, 128);
            return pool;
        }
    }

    internal sealed class HookedComparer : IEqualityComparer<ReadOnlyMemory<char>>
    {
        public PooledDictionary<ReadOnlyMemory<char>, object> Dictionary;
        public HookedComparer(ObjectPool<PooledDictionary<ReadOnlyMemory<char>, object>> pool) => Dictionary = new(pool, this);

        public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y) => ReadOnlyMemoryOfCharComparer.Instance.Equals(x, y);
        public int GetHashCode(ReadOnlyMemory<char> obj)
        {
            if (Dictionary is { Count: 1 } && Dictionary.FirstValue() is SourceOrdinaryMethodSymbolSimple { IsIterator: true, IsStatic: true, IsAsync: false,
                Syntax: { Parent: StructDeclarationSyntax { Members: [_], BaseList.Types: [_] } parent } decl,
                ContainingType: SourceNamedTypeSymbol { IsPartial: false } containingType } method && 
                IsMatch(decl.ReturnType, parent) && containingType.GetDeclaredInterfaces(null) is
                [{ OriginalDefinition.SpecialType: SpecialType.System_Collections_Generic_IEnumerator_T } iEnumeratorT])
            {
                //Set our SpecialType to non-generic IEnumerator, just so that it passes ValidateIteratorMethod
                //Then store the correct element type directly in the method
                method._lazyIteratorElementType = new(iEnumeratorT.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[0]);
                ThreadSafeFlagOperations.Set(ref containingType._flags._flags, (int)SpecialType.System_Collections_IEnumerator);

                //Add the implementation of IEnumerator<T> and its base interfaces
                //Null out the dictionary, so we don't get in a loop
                var dict = Dictionary; Dictionary = null!;

                AddInterfaceMembers(iEnumeratorT);
                foreach (var type in iEnumeratorT.InterfacesNoUseSiteDiagnostics())
                    AddInterfaceMembers(type);

                dict.AddMemberToMultiValueDictionary(new SynthesizedGetEnumeratorMethod(containingType, method));
                Dictionary = dict;

                void AddInterfaceMembers(NamedTypeSymbol type)
                {
                    foreach (var member in type.GetMembers())
                    {
                        if (member.Kind is SymbolKind.Property)
                        {
                            var prop = SynthesizedImplementationProperty.Create(((PropertySymbol)member).GetMethod, containingType);
                            dict.AddMemberToMultiValueDictionary(prop);
                            dict.AddMemberToMultiValueDictionary(prop.GetMethod);
                        }
                        else if ((MethodSymbol)member is { MethodKind: not MethodKind.PropertyGet } m)
                        {
                            dict.AddMemberToMultiValueDictionary(new SynthesizedImplicitImplementationMethod(containingType, m));
                        }
                    }
                }
            }

            return ReadOnlyMemoryOfCharComparer.Instance.GetHashCode(obj);
        }

        private static bool IsMatch(TypeSyntax type, StructDeclarationSyntax parent)
        {
            if (parent.TypeParameterList is not { Parameters: { Count: var count } list })
                return type is IdentifierNameSyntax id && id.Identifier.ValueText == parent.Identifier.ValueText;

            if (type is not GenericNameSyntax { TypeArgumentList.Arguments: { Count: var arity } args } name || arity != count || name.Identifier.ValueText != parent.Identifier.ValueText)
                return false;

            for (int i = 0; i < count; i++)
            {
                if (args[i] is not IdentifierNameSyntax arg || arg.Identifier.ValueText != list[i].Identifier.ValueText)
                    return false;
            }

            return true;
        }
    }
}
