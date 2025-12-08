using System.Data;

namespace UnlimitedSharp
{
    static class Example
    {
        public static void Deconstruct<T1, T2>(this object[] args, out T1 var1, out T2 var2)
        {
            var1 = (T1)args[0];
            var2 = (T2)args[1];
        }

        public static void Constrain<T, U>(T t) where T : IEnumerable<U>
        {
            foreach (var u in t)
                Console.WriteLine(u);
        }

        public static TResult UseValueDelegate<TArg, TResult, TLambda>(TArg u, ref TLambda func)
            where TLambda : struct, IFunc<TArg, TResult> => func.Invoke(u);

        public static void Main(string[] args)
        {
            new Program().Main(args);
        }
    }

    class Program
    {
        public int MethodGroup<T>(T t) { Console.WriteLine(t is null ? "NULL" : typeof(T).ToString()); return 123; }
        public static int StaticMethodGroup<T>(T t) { Console.WriteLine(t is null ? "NULL" : typeof(T).ToString()); return 123; }

        public void Main(string[] args)
        {
            object[] args2 = { "1", 2 };
            (string first, int second) = args2; //inferred Deconstruct<string, int>

            var list = new List<int>();
            Example.Constrain(list); //inferred Constrain<List<int>, int>

            //Non-capturing lambda
            Console.WriteLine(Example.UseValueDelegate(123, static i => i.ToString()));

            //Capturing lambda
            int x = 0;
            Console.WriteLine(Example.UseValueDelegate(42, i => { x = i; return "captured variable modified"; }));
            Console.WriteLine(x.ToString());

            //To test method groups, modify Delegates.tt and PromoteValueDelegates in the analyzer
            //Console.WriteLine(Example.UseValueDelegate("NOT NULL!", MethodGroup));
            //Console.WriteLine(Example.UseValueDelegate(new DataTable(), MethodGroup));
            //Console.WriteLine(Example.UseValueDelegate(123, MethodGroup));

            //Console.WriteLine(Example.UseValueDelegate("NOT NULL!", StaticMethodGroup));
            //Console.WriteLine(Example.UseValueDelegate(new DataTable(), StaticMethodGroup));
            //Console.WriteLine(Example.UseValueDelegate(123, StaticMethodGroup));

        }

    }
}

