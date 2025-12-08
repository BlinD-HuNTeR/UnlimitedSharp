using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using System.Collections.Immutable;

namespace UnlimitedSharp
{
    class ValueDelegateInstrumenter(Instrumenter previous) : CompoundInstrumenter(previous)
    {
        public override CompoundInstrumenter WithPreviousImpl(Instrumenter previous) => new ValueDelegateInstrumenter(previous);
        public override void InterceptCallAndAdjustArguments(ref MethodSymbol method, ref BoundExpression? receiver, ref ImmutableArray<BoundExpression> arguments, ref ImmutableArray<RefKind> argumentRefKindsOpt)
        {
            Previous.InterceptCallAndAdjustArguments(ref method, ref receiver, ref arguments, ref argumentRefKindsOpt);

            for (int i = 0; i < arguments.Length; i++)
                if (arguments[i].Type is { TypeKind: TypeKind.Delegate, IsValueType: true })
                    arguments = arguments.SetItem(i, new BoundInstrumentedExpression(arguments[i]));
        }
    }

    class BoundInstrumentedExpression(BoundExpression target) : BoundExpression(target.Kind, target.Syntax, target.Type, target.HasErrors)
    {
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            var result = visitor.Visit(target);

            if (result is null) 
                return null;

            if (visitor is not ClosureConversion rewriter)
                return Update((BoundExpression)result);

            //Roslyn generates invalid codegen when trying to cache struct delegates, so remove the caching for now
            //Also, invoking a struct's constructor should have almost no overhead.
            if (result is BoundNullCoalescingOperator { RightOperand: BoundAssignmentOperator { Right: var right } })
                result = right;

            return result;
        }

        public BoundInstrumentedExpression Update(BoundExpression updated) => target != updated ? new(updated) : this;
    }
}