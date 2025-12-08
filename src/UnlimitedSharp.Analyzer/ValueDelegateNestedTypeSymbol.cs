using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Roslyn.Utilities;
using System.Collections.Immutable;

namespace UnlimitedSharp;

//In Roslyn, everything is either abstract or sealed. This is a pain in the ass when it comes to hooking things.
//We want to subclass SubstitutedNestedTypeSymbol, but it's sealed. So, we need to subclass the most-derived non-sealed class,
//and reimplement what's missing.
internal sealed class ValueDelegateNestedTypeSymbol : SubstitutedNamedTypeSymbol
{
    internal ValueDelegateNestedTypeSymbol(SubstitutedNamedTypeSymbol newContainer, NamedTypeSymbol originalDefinition)
        : base(newContainer, newContainer.TypeSubstitution, originalDefinition, unbound: newContainer.IsUnboundGenericType && originalDefinition.Arity == 0)
    {
    }

    public override bool IsValueType => true;
    public override bool IsReferenceType => false;
    public override TypeKind TypeKind => TypeKind.Delegate;

    //This is never called unless we wrap this class in another instance of itself
    public override NamedTypeSymbol AsMember(NamedTypeSymbol newOwner) => newOwner.IsDefinition ? this : 
        new ValueDelegateNestedTypeSymbol((SubstitutedNamedTypeSymbol)newOwner, OriginalDefinition);

    public override TypeSymbol SetNullabilityForReferenceTypes(Func<TypeWithAnnotations, TypeWithAnnotations> transform)
    {
        var type = base.SetNullabilityForReferenceTypes(transform);
        return type is ValueDelegateNestedTypeSymbol result ? result : new((SubstitutedNamedTypeSymbol)type.ContainingType, (NamedTypeSymbol)type.OriginalDefinition);
    }

    public override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        => OriginalDefinition.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes);

    public override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics => GetTypeParametersAsTypeArguments();
    public override NamedTypeSymbol ConstructedFrom => this;

    public override bool AreLocalsZeroed => throw ExceptionUtilities.Unreachable();
    public override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData) => throw ExceptionUtilities.Unreachable();
}