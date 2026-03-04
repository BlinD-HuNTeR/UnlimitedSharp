using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace UnlimitedSharp
{
    static class TypeOf<T>
    {
        public static class WithFieldsFrom<U>
        {
            public static readonly IntPtr TypeHandle = CreateType().TypeHandle.Value;
            private static Type CreateType()
            {
                var baseType = typeof(T);
                var type = TypeExtensions.DynamicModule.DefineType(baseType.Name, TypeAttributes.Class | TypeAttributes.Sealed, baseType);

                foreach (var field in typeof(U).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                    type.DefineField(field.Name, field.FieldType, field.Attributes);

                return type.CreateTypeInfo()!;
            }
        }
    }

    static class TypeExtensions
    {
        internal static readonly ModuleBuilder DynamicModule = AssemblyBuilder.DefineDynamicAssembly(
            new("UnspeakableNames"), AssemblyBuilderAccess.Run).DefineDynamicModule("UnspeakableNames.dll");
    }
}