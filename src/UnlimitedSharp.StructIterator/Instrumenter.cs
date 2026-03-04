using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Roslyn.Utilities;
using System.Runtime.CompilerServices;

namespace UnlimitedSharp
{
    internal sealed class StructIteratorInstrumenter(Instrumenter previous) : CompoundInstrumenter(previous)
    {
        public override CompoundInstrumenter WithPreviousImpl(Instrumenter previous) => new StructIteratorInstrumenter(previous);
        public override void PreInstrumentBlock(BoundBlock original, LocalRewriter rewriter)
        {
            //For some reason InstrumentBlock is never called, so we do this in PreInstrumentBlock instead.

            if (original == rewriter.CurrentMethodBody || original == rewriter.CurrentLambdaBody)
                if (rewriter.Factory.CurrentFunction is { IsIterator: true, ReturnType.IsValueType: true })
                    Unsafe.AsRef(in original.Statements.AsSpan()[^1]) = new BoundStructIteratorBody(original.Statements[^1]);
            
            Previous.PreInstrumentBlock(original, rewriter);
        }
    }

    internal sealed class BoundStructIteratorBody(BoundNode wrapped) : BoundStatement(wrapped.Kind, wrapped.Syntax, wrapped.HasErrors)
    {
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            var result = wrapped.Accept(visitor);
            if (visitor is not IteratorMethodToStateMachineRewriter rewriter)
                return Update(result);

            var returnType = rewriter.OriginalMethod.ReturnType;
            var stateMachineType = rewriter.CurrentMethod.ContainingType;

            if (stateMachineType is IteratorStateMachine impl && returnType is { IsValueType: true, OriginalDefinition: SourceNamedTypeSymbol s})
            {
                AddMethodImplementations(s, rewriter.F, impl);
                stateMachineType.GetMethodTable() = TypeOf<StructIteratorStateMachine>.WithFieldsFrom<IteratorStateMachine>.TypeHandle;
            }

            return result;
        }

        public BoundStructIteratorBody? Update(BoundNode? updated) => updated is null ? null : wrapped != updated ? new(updated) : this;

        private static void AddMethodImplementations(SourceNamedTypeSymbol type, SyntheticBoundNodeFactory F, IteratorStateMachine implementingType)
        {
            var oldFunction = F.CurrentFunction;
            var field = new SynthesizedPrivateField(type, implementingType, "_stateMachine");

            //We have to add the field directly to the type instead of using AddSynthesizedDefinition,
            //because otherwise the struct would be emitted with size=1
            type._lazyMembersFlattened.InPlaceAdd(field);

            foreach (var member in type.GetMembers())
            {
                if (member is SynthesizedImplementationAccessor a)
                    GenerateMethodBody(a, a._interfaceMethod);
                else if (member is SynthesizedImplicitImplementationMethod m)
                    GenerateMethodBody(m, m.BaseMethod);
            }

            F.CurrentFunction = oldFunction;

            void GenerateMethodBody(MethodSymbol method, MethodSymbol interfaceMethod)
            {
                F.CurrentFunction = method;
                var invocation = F.Call(F.Field(F.This(), field), interfaceMethod, method.Parameters.SelectAsArray(static BoundExpression (p, F) => F.Parameter(p), F));
                F.CloseMethod(method.ReturnsVoid ? F.Block(F.ExpressionStatement(invocation), F.Return()) : F.Block(F.Return(invocation)));
            }
        }
    }
}