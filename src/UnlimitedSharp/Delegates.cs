using System.Runtime.CompilerServices;
using static System.Runtime.CompilerServices.MethodImplOptions;

namespace UnlimitedSharp;

//Lambdas are always instance methods, method groups might be static.
//At this version I'm not adding support for method groups yet. To add support, modify the "PromoteValueDelegates" method,
//and uncomment the check for null target. However this is not an ideal implementation, we don't want to have branching
//on Delegate.Invoke. A better alternative for static targets could be wrapping a regular delegate in the struct delegate
public interface IFunc<T1, TResult>
{
    public TResult Invoke(T1 arg1);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1) => //target is null ? ((delegate* managed<T1, TResult>)method)(arg1) :
                ((delegate* managed<object, T1, TResult>)method)(target, arg1);
    }
}

public interface IAction<T1>
{
    public void Invoke(T1 arg1);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1) { //if (target is null) ((delegate* managed<T1, void>)method)(arg1); else
                ((delegate* managed<object, T1, void>)method)(target, arg1); }
    }
}

public interface IFunc<T1, T2, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2) => //target is null ? ((delegate* managed<T1, T2, TResult>)method)(arg1, arg2) :
                ((delegate* managed<object, T1, T2, TResult>)method)(target, arg1, arg2);
    }
}

public interface IAction<T1, T2>
{
    public void Invoke(T1 arg1, T2 arg2);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2) { //if (target is null) ((delegate* managed<T1, T2, void>)method)(arg1, arg2); else
                ((delegate* managed<object, T1, T2, void>)method)(target, arg1, arg2); }
    }
}

public interface IFunc<T1, T2, T3, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3) => //target is null ? ((delegate* managed<T1, T2, T3, TResult>)method)(arg1, arg2, arg3) :
                ((delegate* managed<object, T1, T2, T3, TResult>)method)(target, arg1, arg2, arg3);
    }
}

public interface IAction<T1, T2, T3>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3) { //if (target is null) ((delegate* managed<T1, T2, T3, void>)method)(arg1, arg2, arg3); else
                ((delegate* managed<object, T1, T2, T3, void>)method)(target, arg1, arg2, arg3); }
    }
}

public interface IFunc<T1, T2, T3, T4, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => //target is null ? ((delegate* managed<T1, T2, T3, T4, TResult>)method)(arg1, arg2, arg3, arg4) :
                ((delegate* managed<object, T1, T2, T3, T4, TResult>)method)(target, arg1, arg2, arg3, arg4);
    }
}

public interface IAction<T1, T2, T3, T4>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, void>)method)(arg1, arg2, arg3, arg4); else
                ((delegate* managed<object, T1, T2, T3, T4, void>)method)(target, arg1, arg2, arg3, arg4); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, TResult>)method)(arg1, arg2, arg3, arg4, arg5) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5);
    }
}

public interface IAction<T1, T2, T3, T4, T5>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, void>)method)(arg1, arg2, arg3, arg4, arg5); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, void>)method)(target, arg1, arg2, arg3, arg4, arg5); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, T7, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, T7, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6, T7>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6, T7>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6, T7, T8>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6, T7, T8>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15); }
    }
}

public interface IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
{
    public TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe TResult Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) => //target is null ? ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16) :
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
    }
}

public interface IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
{
    public void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16);

    [method: MethodImpl(AggressiveInlining)]
    public struct Delegate(object target, nint method) : IAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>
    {
        [MethodImpl(AggressiveInlining)]
        public unsafe void Invoke(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16) { //if (target is null) ((delegate* managed<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, void>)method)(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16); else
                ((delegate* managed<object, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, void>)method)(target, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16); }
    }
}

