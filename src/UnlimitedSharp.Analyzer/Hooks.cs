using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Microsoft.CodeAnalysis.CSharp.MethodTypeInferrer;

namespace UnlimitedSharp
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Hooks : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [];
        public override void Initialize(AnalysisContext context) { }

        static Hooks()
        {
            InferrerExtensions._default = Extensions.Default;
            Unsafe.AsRef(in Extensions.Default) = new InferrerExtensions();

            //Call the functions at least once to ensure the delegates are initialized
            var inferrer = default(MethodTypeInferrer); var useSiteInfo = default(CompoundUseSiteInfo<AssemblySymbol>);
            try { inferrer.FixNondependentParameters(ref useSiteInfo); } catch { }
            try { inferrer.FixDependentParameters(ref useSiteInfo); } catch { }

            var fields = typeof(MethodTypeInferrer).GetNestedType("<>c", BindingFlags.NonPublic).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(FixParametersPredicate))
                    continue;

                var func = (FixParametersPredicate)field.GetValue(null);
                field.SetValue(null, (FixParametersPredicate)func.Hook);
            }
        }
    }
}
