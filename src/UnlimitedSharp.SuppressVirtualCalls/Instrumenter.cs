using Microsoft.CodeAnalysis.CSharp;

namespace UnlimitedSharp
{
    class SuppressVirtualCallsInstrumenter(Instrumenter previous) : CompoundInstrumenter(previous)
    {
        public override CompoundInstrumenter WithPreviousImpl(Instrumenter previous) => new SuppressVirtualCallsInstrumenter(previous);
        public override BoundExpression InstrumentCall(BoundCall original, BoundExpression rewritten)
        {
            rewritten = Previous.InstrumentCall(original, rewritten);

            if (original is { Method.OriginalDefinition: var outer, Arguments: [BoundCall { Method.OriginalDefinition: var inner }] }
                && (object)outer == inner && outer.Name == "As" && outer.ContainingType.Name == "Unsafe")
            {
                rewritten = new BoundConversion(rewritten.Syntax, rewritten, Conversion.Identity, isBaseConversion: true, false, false, null, null, rewritten.Type!);
            }

            return rewritten;
        }
    }
}