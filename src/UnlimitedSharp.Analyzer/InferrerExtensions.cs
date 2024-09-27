using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using static Microsoft.CodeAnalysis.CSharp.Symbols.SourceLocalSymbol;

namespace UnlimitedSharp
{
    internal class InferrerExtensions : MethodTypeInferrer.Extensions
    {
        internal static MethodTypeInferrer.Extensions _default;
        public override TypeWithAnnotations GetMethodGroupResultType(BoundMethodGroup group, MethodSymbol method)
            => _default.GetMethodGroupResultType(group, method);

        public override TypeWithAnnotations GetTypeWithAnnotations(BoundExpression expr)
            => expr is OutDeconstructVarPendingInference var && var.VariableSymbol is { } symbol && 
            symbol is not DeconstructionLocalSymbol ? TypeWithAnnotations.Create(symbol.GetTypeOrReturnType().Type) :
            _default.GetTypeWithAnnotations(expr);
    }

    internal sealed class InferenceDone : InferrerExtensions
    {
        public static readonly InferenceDone Instance = new();
    }
}