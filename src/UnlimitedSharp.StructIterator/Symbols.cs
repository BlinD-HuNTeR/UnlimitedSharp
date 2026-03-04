using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace UnlimitedSharp
{
    using Microsoft.CodeAnalysis.CSharp;
    using System.Reflection;

    //This class is never instantiated. Instead we mutate the type of an IteratorStateMachine object into this class
    //For this to work properly, this class must have the same fields as IteratorStateMachine.
    //So we dynamically generate a subclass at runtime with the appropriate fields
    internal class StructIteratorStateMachine() : StateMachineTypeSymbol(null, null, null, 0)
    {
        public override TypeKind TypeKind => TypeKind.Struct;
        public override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => (NamedTypeSymbol)DeclaringCompilation.CommonGetSpecialType(SpecialType.System_ValueType);

        public override bool IsRecord => false;
        public override bool IsRecordStruct => false;
        public override bool HasPossibleWellKnownCloneMethod() => false;

        //Non-virtual calls, this requires UnlimitedSharp.SuppressVirtualCalls
        public override MethodSymbol? Constructor => Unsafe.As<IteratorStateMachine>(Unsafe.As<object>(this)).Constructor;
        public override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> _) 
            => Unsafe.As<IteratorStateMachine>(Unsafe.As<object>(this)).InterfacesNoUseSiteDiagnostics(_);

        //Forces an identity conversion to succeed
        public override bool Equals(TypeSymbol t2, TypeCompareKind comparison) => true;
    }

    internal class SynthesizedImplementationProperty : SynthesizedStateMachineProperty, ISynthesizedMethodBodyImplementationSymbol
    {
        public static SynthesizedImplementationProperty Create(MethodSymbol getter, NamedTypeSymbol containingType) =>
            getter.ContainingType.SpecialType is SpecialType.System_Collections_IEnumerator ?
            new SynthesizedImplementationProperty(getter, containingType) :
            new SynthesizedPublicImplementationProperty(getter, containingType);

        public SynthesizedImplementationProperty(MethodSymbol getter, NamedTypeSymbol containingType) : this()
        {
            Unsafe.AsRef(in _getter) = Unsafe.As<SynthesizedStateMachineMethod>(new SynthesizedImplementationAccessor(getter, containingType, this));

            var name = getter.AssociatedSymbol.Name;
            Unsafe.AsRef(in _name) = DeclaredAccessibility is Accessibility.Public ? name : ExplicitInterfaceHelpers.GetMemberName(name, getter.ContainingType, null);
        }

        //We want this class to not call any base class constructor
        //We have an Instrumenter that will remove this constructor and the call to it
        private extern unsafe SynthesizedImplementationProperty();

        bool ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency => false;
        IMethodSymbolInternal ISynthesizedMethodBodyImplementationSymbol.Method => GetMethod;
    }

    internal sealed class SynthesizedPublicImplementationProperty : SynthesizedImplementationProperty
    {
        public SynthesizedPublicImplementationProperty(MethodSymbol getter, NamedTypeSymbol containingType) : base(getter, containingType) 
            => GetMethod = new SynthesizedImplicitImplementationMethod(containingType, getter, this);

        public override MethodSymbol GetMethod { get; }
        public override Accessibility DeclaredAccessibility => Accessibility.Public;
    }

    internal sealed class SynthesizedImplementationAccessor(MethodSymbol interfaceMethod, NamedTypeSymbol containingType, PropertySymbol associatedProperty)
        : SynthesizedImplementationMethod(interfaceMethod, containingType, null, false, associatedProperty)
    {
        public override Location? TryGetFirstLocation() => Location.None;

        public override bool SynthesizesLoweredBoundBody => true;
        public override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            //Do nothing. AddMethodImplementations will generate our body
        }
    }

    internal sealed class SynthesizedImplicitImplementationMethod : SynthesizedMethodBaseSymbol
    {
        public SynthesizedImplicitImplementationMethod(NamedTypeSymbol containingType, MethodSymbol implementedMethod, PropertySymbol? associatedProperty = null)
            : base(containingType, implementedMethod, containingType.GetNonNullSyntaxNode().GetReference(), Location.None, implementedMethod.Name, DeclarationModifiers.Public, false)
        {
            //If you ever want to add support for generic methods, use TypeMap.WithAlphaRename (see BaseMethodWrapperSymbol for an example)
            AssignTypeMapAndTypeParameters(implementedMethod.TypeSubstitution ?? TypeMap.Empty, []);

            state.NotePartComplete(CompletionPart.Attributes | CompletionPart.ReturnTypeAttributes);
            AssociatedSymbol = associatedProperty;
        }

        public override Symbol? AssociatedSymbol { get; }

        public override bool SynthesizesLoweredBoundBody => true;
        public override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            //Do nothing. AddMethodImplementations will generate our body
        }

        public override bool GenerateDebugInfo => false;
        public override ExecutableCodeBinder? TryGetBodyBinder(BinderFactory binderFactoryOpt, bool ignoreAccessibility) => null;
    }

    internal sealed class SynthesizedGetEnumeratorMethod(SourceNamedTypeSymbol containingType, MethodSymbol iteratorMethod)
        : SynthesizedRecordOrdinaryMethod(containingType, "GetEnumerator", 0, DeclarationModifiers.Public)
    {
        public override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            var F = new SyntheticBoundNodeFactory(this, this.GetNonNullSyntaxNode(), compilationState, diagnostics);
            F.CloseMethod(F.Block(F.Return(F.This())));
        }

        public override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.AggressiveInlining;
        public override int GetParameterCountFromSyntax() => 0;

        public override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
            => (iteratorMethod.ReturnTypeWithAnnotations, []);
    }

    internal sealed class SynthesizedPrivateField : SynthesizedFieldSymbolBase
    {
        //The base constructor has changed signature across Roslyn versions, which would cause a MissingMethodException
        //if we tried to call it. So we call no base constructors, and initialize the fields ourselves.
        public SynthesizedPrivateField(NamedTypeSymbol containingType, TypeSymbol fieldType, string name) : this()
        {
            Unsafe.AsRef(in _containingType) = containingType;
            Unsafe.AsRef(in _name) = name;
            Unsafe.AsRef(in _modifiers) = DeclarationModifiers.Private;
            _fieldType = TypeWithAnnotations.Create(fieldType);
        }

        private readonly TypeWithAnnotations _fieldType;
        public override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound) => _fieldType;

        //We want this class to not call any base class constructor
        //We have an Instrumenter that will remove this constructor and the call to it
        private extern unsafe SynthesizedPrivateField();

        public override RefKind RefKind => RefKind.None;
        public override ImmutableArray<CustomModifier> RefCustomModifiers => [];
        public override bool SuppressDynamicAttribute => true;

    }
}