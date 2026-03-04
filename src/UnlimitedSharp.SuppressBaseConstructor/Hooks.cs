using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using static Microsoft.CodeAnalysis.CSharp.MethodTypeInferrer;

namespace UnlimitedSharp
{
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Symbols;

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Hooks : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray<DiagnosticDescriptor>.Empty;
        public override void Initialize(AnalysisContext context) { }

        static Hooks()
        {
            var fields = typeof(MethodCompiler).GetNestedType("<>c", BindingFlags.NonPublic).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

            foreach (var field in fields.OfFieldType<Func<Binder, CSharpSyntaxNode, object, BindingDiagnosticBag, BoundNode>>())
                field.SetValue(null, (field.GetValue(null) ?? Original).Hook);
        }

        static BoundNode Original(Binder binder, CSharpSyntaxNode syntaxNode, object _, BindingDiagnosticBag diagnostics) => binder.BindMethodBody(syntaxNode, diagnostics);
    }

    static class Extensions
    {
        private const DeclarationModifiers PrivateExternUnsafe = DeclarationModifiers.Private | DeclarationModifiers.Extern | DeclarationModifiers.Unsafe;
        public static BoundNode Hook(this Func<Binder, CSharpSyntaxNode, object, BindingDiagnosticBag, BoundNode> original, Binder binder, CSharpSyntaxNode syntaxNode, object _, BindingDiagnosticBag diagnostics)
        {
            var result = original(binder, syntaxNode, _, diagnostics);
            if (result is BoundConstructorMethodBody { Initializer: BoundExpressionStatement { Expression: BoundCall { Method: SourceConstructorSymbol constructor } } } body
                && (constructor.DeclarationModifiers & PrivateExternUnsafe) == PrivateExternUnsafe)
            {
                //By setting it to partial, it won't be emitted
                Unsafe.AsRef(in constructor.DeclarationModifiers) |= DeclarationModifiers.Partial;

                //Also, remove the initializer call from the constructor body
                Unsafe.AsRef(in body.'<Initializer>k__BackingField') = null;
            }

            return result;
        }

        public static IEnumerable<Field<T>> OfFieldType<T>(this FieldInfo[] fields)
        {
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(T))
                    yield return new(field);
            }
        }
    }

    struct Field<T>(FieldInfo field)
    {
        public T GetValue(object? obj) => (T)field.GetValue(obj)!;
        public void SetValue(object? obj, T value) => field.SetValue(obj, value);
    }
}
