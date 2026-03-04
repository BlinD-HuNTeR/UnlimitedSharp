using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace UnlimitedSharp
{
    //DO NOT USE THIS PROJECT!!! It's unstable and doesn't always work
    //The problem is that analyzers are only loaded AFTER syntax trees have been parsed, so our parser hooks never get a chance to run
    //If you are building with VBCSCompiler, then the first build will fail, but others will succeed, as long as VBCSCompiler keeps running
    //If you build with csc.exe (UseSharedCompilation=false), then it never works

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Hooks : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [];
        public override void Initialize(AnalysisContext context) { }

        [ModuleInitializer]
        internal static void Init()
        {
            //If this assembly is loaded before LanguageParser has been used, the cached delegates will not have been created yet.
            //TODO Figure a way to force their creation.

            //var parser = new LanguageParser(null!, null, null);
            //try { parser.ParseCompilationUnit(); } catch { }

            //Patch all cached delegates of type Func<LanguageParser, CompilationUnitSyntax>
            var fields = typeof(LanguageParser).GetNestedType("<>c", BindingFlags.NonPublic).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.FieldType != typeof(Func<LanguageParser, CompilationUnitSyntax>))
                    continue;

                var func = (Func<LanguageParser, CompilationUnitSyntax>)field.GetValue(null);
                field.SetValue(null, (Func<LanguageParser, CompilationUnitSyntax>)func.Hook);
            }
        }
    }
}
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ModuleInitializerAttribute : Attribute { }
}
