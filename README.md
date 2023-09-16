NamedResolver
=============

> ⚠️Since AspNetCore 8.0 now implements [keyed DI services](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8#keyed-di-services) ❤️, this library goes to public archive :) but still available in [NuGet](https://www.nuget.org/packages/NamedResolver)!

An abstraction that provide ability to use interface with multiple implementations or preconfigured instances in easy way. Drop in replacement for simple switch case factories.

[![NuGet version (NamedResolver)](https://img.shields.io/nuget/v/NamedResolver.svg?style=flat-square)](https://www.nuget.org/packages/NamedResolver)
![UnitTest](https://github.com/mt89vein/NamedResolver/workflows/UnitTest/badge.svg)
[![Coverage Status](https://coveralls.io/repos/github/mt89vein/NamedResolver/badge.svg?branch=master)](https://coveralls.io/github/mt89vein/NamedResolver?branch=master)

Look at tests for additional examples.

### Installing NamedResolver

You should install [NamedResolver with NuGet](https://www.nuget.org/packages/NamedResolver):

    Install-Package NamedResolver -Version 2.2.0
    
via the .NET Core command line interface:

    dotnet add package NamedResolver --version 2.2.0
    
or package reference

    <PackageReference Include="NamedResolver" Version="2.2.0" />
    
## Use cases

let's assume we have this:

```csharp
public interface ISomeInterface {}

public class FirstImplementation : ISomeInterface {}

public class SecondImplementation : ISomeInterface {}
```

And register in dependency injection container like this:

```csharp

services.AddScoped<ISomeInterface, FirstImplementation>();
services.AddScoped<ISomeInterface, SecondImplementation>();

```

And sometimes we need to resolve one of implementation of interface directly.

In most cases, for resolve, we inject `IEnumerable<TInterface>` in our DependentClass
and then search using typeof(T).

```csharp
public class DependentClass
{
    private readonly ISomeInterface _firstImplementation;

    public DependentClass(IEnumerable<ISomeInterface> implementations)
    {
        // in this case we always resolve from DI all implementations of ISomeInterface.
        _firstImplementation =
            implementations.SingleOrDefault(i => i.GetType() == typeof(FirstImplementation)) ??
            throw new ArgumentNullException($"Cannot resolve instance of type {typeof(FirstImplementation).FullName}");
    }
}
```

Also we can register to IServiceCollection our implementation without interface,
and inject implementation into DependenentClass explicitly, but it is not good for unit testing:

```csharp

services.AddScoped<FirstImplementation>();
services.AddScoped<SecondImplementation>();

```

```csharp
public class DependentClass
{
    private readonly ISomeInterface _firstImplementation;

    public DependentClass(FirstImplementation firstImplementation)
    {
        _firstImplementation = firstImplementation;
    }
}
```


Same with NamedResolver

```csharp

services.AddNamed<string, ISomeInterface>(ServiceLifeTime.Scoped)
        .Add<FirstImplementation>("First");
        .Add<SecondImplementation>("Second");

```

```csharp
public class DependentClass
{
    private readonly ISomeInterface _firstImplementation;

    public DependentClass(INamedResolver<string, ISomeInterface> resolver)
    {
        _firstImplementation = resolver.Get("First");
    }
}
```


## Features:

### Default instance:

```csharp
public class DefaultImplementation : ISomeInterface {}
```

```csharp

services.AddNamed<string, ISomeInterface>(ServiceLifeTime.Scoped)
        .Add<FirstImplementation>("First");
        .Add<SecondImplementation>("Second")
        .Add<DefaultImplementation>(); // default - without name parameter

```

```csharp

public class DependentClass
{
    private readonly ISomeInterface _defaultImplementation;

    public DependentClass(ISomeInterface someInterface)
    {
        // DefaultImplementation would be injected
        _defaultImplementation = someInterface;
    }
}

```

### Preconfigured instances

```csharp

services.AddNamed<string, ISomeInterface>(ServiceLifeTime.Scoped)
        // note that such factory registration will not respect lifetime scope, and will be created every time!
        .Add<FirstImplementation>("FirstUseCase", _ => new FirstImplementation("FirstUseCase"));
        .Add<FirstImplementation>("SecondUseCase", _ => new FirstImplementation("SecondUseCase"));

```

### Enum or custom class as discriminator

```csharp

public enum MyEnum
{
    Default = 0,
    FirstUseCase = 1,
    SecondUseCase = 2
}

services.AddNamed<MyEnum, ISomeInterface>()
        .Add<FirstImplementation>(MyEnum.FirstUseCase)
        .Add<SecondImplementation>(MyEnum.SecondUseCase)
        .Add<DefaultImplementation>(); // or MyEnum.Default

```

for correct search type by class you also should implement IEquatable<T>
or IEqualityComparer<T> and provide it to AddNamed method.
By default used EqualityComparer<T>.Default

### Resolve all

```csharp

public class DependentClass
{
    private readonly IEnumerable<ISomeInterface> _implementations;
    private readonly IEnumerable<ISomeInterface> _fromSampleNamespace;
    private readonly IEnumerable<(string name, ISomeInterface instance)> _implementationsWithNames;

    public DependentClass(INamedResolver<string, ISomeInterface> resolver)
    {
        _implementations = resolver.GetAll();
        _implementationsWithNames = resolver.GetAllWithNames();
        _fromSampleNamespace = resolver.GetAll(t => t.Namespace.StartsWith("Sample"));
    }
}

```
or with IReadOnlyList<T>. With injecting IEnumerable<T> you get only default implementation or InvalidOperationException.
```csharp

public class DependentClass
{
    private readonly IReadOnlyList<ISomeInterface> _implementations;

    // same result as INamedResolver<string, ISomeInterface>.GetAll method
    public DependentClass(IReadOnlyList<ISomeInterface> implementations)
    {
        _implementations = implementations;
    }
}

```

### Resolve with delegate

```csharp

public delegate TInterface ResolveNamed<in TDiscriminator, out TInterface>(
    TDiscriminator name = default
)

public class SomeClass
{
    private readonly ISomeInterface _implementation;

    public SomeClass(ResolveNamed<string, ISomeInterface> resolveNamedFunc)
    {
        _implementation = resolveNamedFunc("Test");
	}
}
```



### Safe TryAdd both generic/non-generic method

```csharp

services.AddNamed<string, ISomeInterface>(ServiceLifeTime.Scoped)
        .Add<FirstImplementation>("FirstUseCase", _ => new FirstImplementation("FirstUseCase"));
        // instance with name "FirstUseCase" already registered above,
        // this TryAdd with same name has no effect.
        .TryAdd<FirstImplementation>("FirstUseCase", _ => new FirstImplementation("FirstUseCase_skipped"));

```

### TryAdd use case

this method would be usefull for other libraries.
Sometimes we want to register some default implementation after user configure callback call, without exceptions or unwanted replaces with unexpected behaviour:

```csharp

#region library code

public class SomeLibraryOptions
{
    public INamedRegistratorBuilder<string, ISomeInterface> SomeInterfaceRegistrator { get; }

    public SomeLibraryOptions(INamedRegistratorBuilder<string, ISomeInterface> registratorBuilder)
    {
        SomeInterfaceRegistrator = registratorBuilder;
    }
}

public static class SomeLibraryServiceCollectionExtensions
{
    public static IServiceCollection AddSomeLibrary(this IServiceCollection services, Action<SomeLibraryOptions> configure = null)
    {
        // take a builder reference
        var builder = services.AddNamed<string, ISomeInterface>();

        // init library options with builder
        var options = new SomeLibraryOptions(builder);

        // let call configure options callback with user code if it not null
        configure?.Invoke(options);

        // try to add default implementation, if user not configured it.
        builder.TryAdd<DefaultImplementation>();

        return services;
    }
}

#endregion library code

#region user code

services.AddSomeLibrary((options) =>
{
    options.SomeInterfaceRegistrator
           .Add<UserCustomImplementation>(); // registered as default.
});

#endregion user code

```

### Contribute

Feel free for creation issues, or PR :)

