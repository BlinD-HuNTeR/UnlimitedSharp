using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("StructIterator")]
namespace UnlimitedSharp
{
    internal static class TypeExtensions
    {
        private class RawData<T> { public required T Data; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref T GetRawData<T>(this object obj) => ref Unsafe.As<RawData<T>>(obj).Data;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref IntPtr GetMethodTable(this object obj) => ref Unsafe.Add(ref obj.GetRawData<IntPtr>(), -1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ChangeType<T>(this object obj) => obj.GetMethodTable() = TypeOf<T>.TypeHandle;

    }

    internal static class TypeOf<T>
    {
        public static readonly IntPtr TypeHandle = typeof(T).TypeHandle.Value;
        public static class WithFieldsFrom<U>
        {
            public static readonly Type Type = CreateType();
            public static readonly IntPtr TypeHandle = Type.TypeHandle.Value;
            private static Type CreateType()
            {
                var baseType = typeof(T);
                var type = DynamicAssembly.DynamicModule.DefineType(baseType.Name, TypeAttributes.Class | TypeAttributes.Sealed, baseType);

                foreach (var field in typeof(U).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                    type.DefineField(field.Name, field.FieldType, field.Attributes);

                return type.CreateTypeInfo()!;
            }
        }
    }

    internal static class DynamicAssembly
    {
        internal static readonly ModuleBuilder DynamicModule = CreateDynamicModule();
        private static ModuleBuilder CreateDynamicModule()
        {
            var assembly = AssemblyBuilder.DefineDynamicAssembly(new("StructIterator"), AssemblyBuilderAccess.Run);
            assembly.SetCustomAttribute(new(typeof(IgnoresAccessChecksToAttribute).GetConstructors()[0], ["Microsoft.CodeAnalysis.CSharp"]));

            return assembly.DefineDynamicModule("StructIterator.dll");
        }
    }
}