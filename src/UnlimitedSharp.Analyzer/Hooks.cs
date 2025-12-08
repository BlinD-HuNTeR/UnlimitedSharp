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

        //In this project, I am refraining from using native hooking solutions, such as MonoMod.RuntimeDetour. I
        //am going to use only purely managed approaches. This means that I can only hook cached delegates, and
        //virtual methods from singleton objects
        static Hooks()
        {
            InferrerExtensions._default = Extensions.Default;
            Unsafe.AsRef(in Extensions.Default) = new InferrerExtensions();

            Unsafe.AsRef(in Instrumenter.NoOp) = new ValueDelegateInstrumenter(Instrumenter.NoOp);
            Unsafe.AsRef(in DebugInfoInjector.s_singleton) = new(Instrumenter.NoOp);

            //Call the functions at least once to ensure the delegates are initialized
            var inferrer = default(MethodTypeInferrer); var useSiteInfo = default(CompoundUseSiteInfo<AssemblySymbol>);
            try { inferrer.FixNondependentParameters(ref useSiteInfo); } catch { }
            try { inferrer.FixDependentParameters(ref useSiteInfo); } catch { }

            //Patch all cached delegates of type FixParametersPredicate
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
