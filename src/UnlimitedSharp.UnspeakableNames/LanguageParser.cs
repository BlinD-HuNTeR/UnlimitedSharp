using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace UnlimitedSharp
{
    using Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax;

    static class LanguageParserHook
    {
        internal static CompilationUnitSyntax Hook(this Func<LanguageParser, CompilationUnitSyntax> original, LanguageParser @this)
        {
            @this.GetMethodTable() = TypeOf<HookedLanguageParser>.WithFieldsFrom<LanguageParser>.TypeHandle;
            return original(@this);
        }
    }

    public class HookedLanguageParser : SyntaxParser
    {
        public override TNode WithAdditionalDiagnostics<TNode>(TNode node, params DiagnosticInfo[] diagnostics)
        {
            if (CurrentToken.Kind is SyntaxKind.CharacterLiteralToken && node.IsMissing && diagnostics is [{ Code: (int)ErrorCode.ERR_IdentifierExpected }] &&
                CurrentToken.GetDiagnostics() is [] or [{ Code: (int)ErrorCode.ERR_TooManyCharsInConst }])
                return (TNode)(GreenNode)SyntaxFactory.Identifier(
                    SyntaxKind.IdentifierToken,
                    CurrentToken.GetLeadingTrivia(),
                    CurrentToken.Text,
                    CurrentToken.Text[1..^1],
                    EatToken().GetTrailingTrivia());

            return base.WithAdditionalDiagnostics(node, diagnostics);
        }

        public HookedLanguageParser() : base(null, default, null, null, false)
        {

        }
    }
}
