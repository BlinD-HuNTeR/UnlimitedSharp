This package enables the use of `yield return` in methods that return struct types (zero-allocation iterators).
To use it, you just need to declare a struct with a static method, that cointains `yield` statements, and whose return type is the struct itself.
The struct must be **non-partial**, and it must contain **no members other than the static iterator method**. It must also implement the `IEnumerator<T>` interface, and no other interfaces.
The static iterator method can have any name, but it must be **non-async** and **non-generic** (the containing struct can be generic).
```
public struct MyIterator : IEnumerator<int>
{
    public static MyIterator Create()
    {
        yield return 1;
        yield return 2;
        yield return 3;
    }
}
```
The compiler will then generate the following code, declaring a private field in the struct for the iterator
state machine, and generating the `IEnumerator<T>` implementation members, delegating to that field.
The internal state machine type will also be generated as a struct, so no allocations will happen:
```
public struct MyIterator : IEnumerator<int>
{
    private MyIterator.<Create>d__0 _stateMachine;

    public int Current => _stateMachine.Current;
    public void Dispose() => _stateMachine.Dispose();
    public bool MoveNext() => _stateMachine.MoveNext();
    public void Reset() => _stateMachine.Reset();
    object IEnumerator.Current => ((IEnumerator)_stateMachine).Current;

    public MyIterator GetEnumerator() => this;

    private struct <Create>d__0 : IEnumerator<int>
    {
        // state machine implementation
    }
}
```

