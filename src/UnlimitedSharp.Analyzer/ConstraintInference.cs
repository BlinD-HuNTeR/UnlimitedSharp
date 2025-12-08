using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static Microsoft.CodeAnalysis.CSharp.MethodTypeInferrer;

namespace UnlimitedSharp
{
    static class ConstraintInference
    {
        internal static bool Hook(this FixParametersPredicate original, ref MethodTypeInferrer inferrer, int index)
        {
            if (inferrer._extensions == InferenceDone.Instance)
                return false;

            if (inferrer._conversions is not Conversions conversions)
                return original(ref inferrer, index);

            return Inferrer.InferTypeArgs(conversions._binder, ref inferrer);
        }
    }

    struct Inferrer
    {
        MethodTypeInferrer self;

        [UnscopedRef] ref ImmutableArray<BoundExpression> _arguments => ref Unsafe.AsRef(in self._arguments);
        [UnscopedRef] ref ImmutableArray<TypeWithAnnotations> _formalParameterTypes => ref Unsafe.AsRef(in self._formalParameterTypes);
        [UnscopedRef] ref ImmutableArray<RefKind> _formalParameterRefKinds => ref Unsafe.AsRef(in self._formalParameterRefKinds);
        [UnscopedRef] ref Extensions _extensions => ref Unsafe.AsRef(in self._extensions);

        internal static bool InferTypeArgs(Binder binder, ref MethodTypeInferrer inferrer)
        {
            var useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            var promotedConstraints = BitVector.Create(inferrer._methodTypeParameters.Length);

            ref var hook = ref Unsafe.As<MethodTypeInferrer, Inferrer>(ref inferrer);
            var promotedArgs = hook._formalParameterRefKinds.IsDefault ? default : hook.PromoteValueDelegates(binder, stackalloc int[hook.self.NumberArgumentsToProcess]);

            if (promotedArgs.Length > 0) hook.InferTypeArgsFirstPhase(binder, ref useSiteInfo, promotedArgs.Length);
            var result = hook.InferTypeArgsSecondPhase(binder, ref useSiteInfo, ref promotedConstraints, promotedArgs);

            hook._extensions = InferenceDone.Instance;
            return result;
        }

        private void InferTypeArgsFirstPhase(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, int start)
        {
            // If we've successfully promoted any constraints, we have to invoke phase 1 for them.
            for (int arg = start; arg < self.NumberArgumentsToProcess; arg++)
            {
                BoundExpression argument = _arguments[arg];
                TypeWithAnnotations target = _formalParameterTypes[arg];
                ExactOrBoundsKind kind = self.GetRefKind(arg).IsManagedReference() || target.Type.IsPointerType() ? ExactOrBoundsKind.Exact : ExactOrBoundsKind.LowerBound;

                self.MakeExplicitParameterTypeInferences(binder, argument, target, kind, ref useSiteInfo);
            }
        }

        private bool InferTypeArgsSecondPhase(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ref BitVector promotedConstraints, Span<int> promotedArgs)
        {
            while (true)
            {
                var res = DoSecondPhase(binder, ref useSiteInfo, ref promotedConstraints, promotedArgs);
                if (res == InferenceResult.InferenceFailed) return false;
                if (res == InferenceResult.Success) return true;
            }
        }

        private InferenceResult DoSecondPhase(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ref BitVector promotedConstraints, Span<int> promotedArgs)
        {
            if (self.AllFixed()) return InferenceResult.Success;
            self.MakeOutputTypeInferences(binder, ref useSiteInfo);

            var res = self.FixParameters((ref MethodTypeInferrer inferrer, int index) => !inferrer.DependsOnAny(index), ref useSiteInfo);
            if (res != InferenceResult.NoProgress) return res;

            res = self.FixParameters((ref MethodTypeInferrer inferrer, int index) => inferrer.AnyDependsOn(index), ref useSiteInfo);
            if (res != InferenceResult.NoProgress) return res;

            res = PromoteGenericConstraintsToFakeArguments(binder, ref useSiteInfo, ref promotedConstraints, promotedArgs);
            if (res != InferenceResult.NoProgress) return res;

            return InferenceResult.InferenceFailed;
        }

        private InferenceResult PromoteGenericConstraintsToFakeArguments(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ref BitVector promotedConstraints, Span<int> promotedArgs)
        {
            int oldArgLength = self.NumberArgumentsToProcess;

            for (int index = 0; index < self._methodTypeParameters.Length; index++)
            {
                // We can only promote the constraints of parameters that have been fixed.
                // We also must not promote the constraints of a parameter more than once.
                if (self.IsUnfixed(index) || promotedConstraints[index])
                    continue;

                // We add a fake argument for every constraint type of a type parameter.
                BoundExpression? fakeArgument = null;
                foreach (var constraintType in self._methodTypeParameters[index].ConstraintTypesNoUseSiteDiagnostics)
                {
                    // Consider the following example:
                    // 1. "where TCollection : IEnumerable<TItem>" constraint;
                    // 2. It's inferred that TCollection is List<int>.
                    // We're going to add a fake "IEnumerable<TItem>" argument and pass "default( List<int> )" to it.
                    fakeArgument ??= BindFakeArgument(self._fixedResults[index].Type.Type);
                    InsertFakeArgument(fakeArgument, constraintType);
                }

                promotedConstraints[index] = true;
            }

            //Now check args promoted to struct lambdas and "promote them back", if they have been inferred
            if (oldArgLength == self.NumberArgumentsToProcess) for (int i = 0; i < promotedArgs.Length; i++)
            {
                int iParam = promotedArgs[i];
                if (iParam > 0 && !HasUnfixedParam(_formalParameterTypes[iParam].Type))
                {
                    var type = self.GetFixedDelegateOrFunctionPointer(_formalParameterTypes[iParam].Type);
                    Debug.Assert(type is ValueDelegateNestedTypeSymbol);

                    InsertFakeArgument(BindFakeArgument(type.OriginalDefinition is ValueDelegateNestedTypeSymbol original ? original : type), _formalParameterTypes[i]);
                    promotedArgs[i] = 0;
                }
            }

            if (oldArgLength == self.NumberArgumentsToProcess) return InferenceResult.NoProgress;

            InferTypeArgsFirstPhase(binder, ref useSiteInfo, oldArgLength);
            return InferenceResult.MadeProgress;
        }

        private static readonly LiteralExpressionSyntax defaultLiteral = SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);
        private static BoundExpression BindFakeArgument(TypeSymbol fixedType) => new BoundDefaultExpression(defaultLiteral, fixedType) { WasCompilerGenerated = true };

        private void InsertFakeArgument(BoundExpression argument, TypeWithAnnotations parameterType)
        {
            int index = self.NumberArgumentsToProcess;

            _arguments.InPlaceInsert(index, argument);
            _formalParameterTypes.InPlaceInsert(index, parameterType);

            if (!_formalParameterRefKinds.IsDefault)
                _formalParameterRefKinds.InPlaceInsert(index, RefKind.None);
        }

        //Possible optimization: inline this function into the caller, and do the stackalloc only when necessary
        //Also possible: do the loop backwards, and stackalloc to the size of the last promoted index
        private Span<int> PromoteValueDelegates(Binder binder, Span<int> promotedArgs)
        {
            int argLength = promotedArgs.Length;
            var originalRefKinds = self._formalParameterRefKinds;

            //In the first implementation we required that Value Delegates be passed by ref. This is not really needed.
            //The "value delegate" is not the closure itself, it's a struct that contains a reference to the closure.
            //Also we should make it work without generics, remove the IFunc interface and all of this generic stuff
            for (int index = 0; index < promotedArgs.Length; index++)
            {
                //To test support for MethodGroups, uncomment the "or BoundMethodGroup", and the null check in the Delegate templates

                if (originalRefKinds[index] is RefKind.Ref && _arguments[index] is UnboundLambda //or BoundMethodGroup
                    && _formalParameterTypes[index] is { Type: TypeParameterSymbol param } type && self.IsUnfixedTypeParameter(type)
                    && param is { HasValueTypeConstraint: true, ConstraintTypesNoUseSiteDiagnostics: [{ Type: { TypeKind: TypeKind.Interface } intf }] } 
                    && intf.GetTypeMembers() is [SubstitutedNestedTypeSymbol { TypeKind: TypeKind.Struct } del])
                {
                    int i = GetOrdinal(param);
                    self._exactBounds[i] = self._lowerBounds[i] = self._upperBounds[i] = null;

                    //We wrap this class in another instance of it, so we can intercept the call to AsMember. It will be unwrapped later
                    var valueType = new ValueDelegateNestedTypeSymbol((SubstitutedNamedTypeSymbol)del.ContainingType, del.OriginalDefinition);
                    InsertFakeArgument(_arguments[index], TypeWithAnnotations.Create(new ValueDelegateNestedTypeSymbol((SubstitutedNamedTypeSymbol)del.ContainingType, valueType)));

                    originalRefKinds.Ref(index) = RefKind.None;
                    promotedArgs[index] = argLength++;
                }
            }

            return argLength == promotedArgs.Length ? default : promotedArgs;
        }

        private bool HasUnfixedParam(TypeSymbol type)
        {
            for (int i = 0; i < self._methodTypeParameters.Length; i++)
            {
                if (self.IsUnfixed(i) && type.ContainsTypeParameter(self._methodTypeParameters[i]))
                    return true;
            }
            return false;
        }

        private int GetOrdinal(TypeParameterSymbol typeParameter)
        {
            //Forward compatibility with extension types (14), while still working in older compiler versions
            if (self._constructedContainingTypeOfMethod.TypeKind != (TypeKind)14)
                return typeParameter.Ordinal;

            //Extension types mess up with ordinals, so we must do manual lookup
            //Roslyn makes a dictionary from TypeParameter to int, but plain IndexOf should be fine IMO
            for (int i = 0; i < self._methodTypeParameters.Length; i++)
                if (ReferenceEquals(typeParameter, self._methodTypeParameters[i]))
                    return i;

            throw new KeyNotFoundException();
        }
    }
}