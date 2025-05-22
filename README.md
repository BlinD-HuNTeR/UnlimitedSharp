# UnlimitedSharp

Just taking notes for myself of the possible next features of this project and the implementation approach:

- Extension everything (more powerful than in C# 14, extensions can even take precedence over instance members)
  - Implementation is be done through IL Weaving (see POC repository), but doesn't work on corelib types, except with compiler patches
  
- Intercept everything (intercept property, constructor, even implicit calls like `GetEnumerator` and `Deconstruct`)
  - Can probably be done with analyzer using private Roslyn API (`Instrumenter`), no need for compiler patch
  
- Deconstruct everything (deconstruction by name, like JS)
  - Probably a mix of source generator and type-inference magic, hooking into `MethodTypeInferrer`, like in the GenericInference project.
  
- Struct lambdas (`ref T where T: struct, IFunc<string, string>`)
  - Hook into `MethodTypeInferrer` to infer `T` as the generated closure class. Then closure class is changed to struct and implements `IFunc<...>`.
    ~Requires hooks in lots of places (binding/lowering/emission), some hooks could be done with analyzer but probably lots of compiler patches are required.~
    It turns out this is easier than I expected, and won't require compiler patches. Preliminar version with delegate as struct (but closure still as class) should be coming soon.

- Named params a.k.a. KwArgs (Allow a method to take arbitrary named arguments, which then become a `Dictionary<string, ...>`)
  - Probably with crazy amounts of source generation, of all the proposals this is probably the less hacky.

- Pipe operator/chaining method calls (https://github.com/dotnet/csharplang/discussions/96)
  - With Extension Everything, it should be possible to add `operator |` to delegate types, then take advantage of delegate natural typing, and it should be possible to write `"Hello World" | Console.WriteLine`
