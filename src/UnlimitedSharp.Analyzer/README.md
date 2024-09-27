This packages enables the inference of generic type parameters from the constraints of other type parameters, as well as inference from deconstruction variables.
Just install the package and the following code, otherwise invalid, magically starts compiling and working as expected:

```
static class Example
{
    static void Deconstruct<T1, T2>(this object[] args, out T1 var1, out T2 var2)
    {
        var1 = (T1)args[0];
        var2 = (T2)args[1];
    }

    static void Constrain<T, U>(T t) where T : IEnumerable<U>
    {
        foreach (var u in t)
            Console.WriteLine(u);
    }

    static void Test()
    {
        object[] args = { "1", 2 };
        (string first, int second) = args; //inferred Deconstruct<string, int>

        var list = new List<int>();
        Constrain(list); //inferred Constrain<List<int>, int>
    }
}
```