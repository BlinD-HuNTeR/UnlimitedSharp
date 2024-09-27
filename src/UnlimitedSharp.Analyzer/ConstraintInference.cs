using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
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
            var result = hook.InferTypeArgsSecondPhase(binder, ref useSiteInfo, ref promotedConstraints);

            hook._extensions = InferenceDone.Instance;
            return result;
        }

        private bool InferTypeArgsSecondPhase(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ref BitVector promotedConstraints)
        {
            while (true)
            {
                var res = DoSecondPhase(binder, ref useSiteInfo, ref promotedConstraints);
                if (res == InferenceResult.InferenceFailed) return false;
                if (res == InferenceResult.Success) return true;
            }
        }

        private InferenceResult DoSecondPhase(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ref BitVector promotedConstraints)
        {
            if (self.AllFixed()) return InferenceResult.Success;
            self.MakeOutputTypeInferences(binder, ref useSiteInfo);

            var res = self.FixParameters((ref MethodTypeInferrer inferrer, int index) => !inferrer.DependsOnAny(index), ref useSiteInfo);
            if (res != InferenceResult.NoProgress) return res;

            res = self.FixParameters((ref MethodTypeInferrer inferrer, int index) => inferrer.AnyDependsOn(index), ref useSiteInfo);
            if (res != InferenceResult.NoProgress) return res;

            res = PromoteGenericConstraintsToFakeArguments(binder, ref useSiteInfo, ref promotedConstraints);
            if (res != InferenceResult.NoProgress) return res;

            return InferenceResult.InferenceFailed;
        }

        private InferenceResult PromoteGenericConstraintsToFakeArguments(Binder binder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ref BitVector promotedConstraints)
        {
            int argLength, oldArgLength;
            argLength = oldArgLength = self.NumberArgumentsToProcess;

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
                    _arguments.InPlaceInsert(argLength, fakeArgument);
                    _formalParameterTypes.InPlaceInsert(argLength, constraintType);

                    if (!_formalParameterRefKinds.IsDefault)
                        _formalParameterRefKinds.InPlaceInsert(argLength, RefKind.None);

                    argLength++;
                }

                promotedConstraints[index] = true;
            }

            if (argLength == oldArgLength) return InferenceResult.NoProgress;

            // If we've successfully promoted any constraints, we have to invoke phase 1 for them.
            for (int arg = oldArgLength; arg < argLength; arg++)
            {
                BoundExpression argument = _arguments[arg];
                TypeWithAnnotations target = _formalParameterTypes[arg];
                ExactOrBoundsKind kind = self.GetRefKind(arg).IsManagedReference() || target.Type.IsPointerType() ? ExactOrBoundsKind.Exact : ExactOrBoundsKind.LowerBound;

                self.MakeExplicitParameterTypeInferences(binder, argument, target, kind, ref useSiteInfo);
            }

            return InferenceResult.MadeProgress;
        }

        private static readonly LiteralExpressionSyntax defaultLiteral = SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression);
        private static BoundExpression BindFakeArgument(TypeSymbol fixedType) => new BoundDefaultExpression(defaultLiteral, fixedType) { WasCompilerGenerated = true };
    }
}