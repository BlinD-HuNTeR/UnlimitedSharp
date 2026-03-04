using System.Runtime.CompilerServices;

public static class Extensions
{
    private class RawData<T> { public required T Data; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref T GetRawData<T>(this object obj) => ref Unsafe.As<RawData<T>>(obj).Data;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ref IntPtr GetMethodTable(this object obj) => ref Unsafe.Add(ref obj.GetRawData<IntPtr>(), -1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ChangeType<T>(this object obj) => obj.GetMethodTable() = typeof(T).TypeHandle.Value;
}
