using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace UnlimitedSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Hooks : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray<DiagnosticDescriptor>.Empty;
        public override void Initialize(AnalysisContext context) { }

        static Hooks()
        {
            Unsafe.AsRef(in DebugInfoInjector.s_singleton) = new(
                Unsafe.AsRef(in Instrumenter.NoOp) = new SuppressVirtualCallsInstrumenter(Instrumenter.NoOp));
        }
    }
}
