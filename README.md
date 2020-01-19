# NamedResolver

An abstraction that provide ability to use interface with multiple implementations or preconfigured instances in easy way using local ServiceLocator pattern.

## Use cases

let's assume we have this:

```csharp
public interface ISomeInterface {}

public class FirstImplementation : ISomeInterface {}

public class SecondImplementation : ISomeInterface {}
```

And we register like this:

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

services.AddNamed<ISomeInterface>(ServiceLifeTime.Scoped)
        .Add<FirstImplementation>("First");
        .Add<SecondImplementation>("Second");

```

```csharp
public class DependentClass
{
    private readonly ISomeInterface _firstImplementation;

    public DependentClass(INamedResolver<ISomeInterface> resolver)
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

services.AddNamed<ISomeInterface>(ServiceLifeTime.Scoped)
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

services.AddNamed<ISomeInterface>(ServiceLifeTime.Scoped)
        .Add<FirstImplementation>("FirstUseCase", _ => new FirstImplementation("FirstUseCase"));
        .Add<FirstImplementation>("SecondUseCase", _ => new FirstImplementation("SecondUseCase"));

```

### Resolve all

```csharp

public class DependentClass
{
    private readonly IEnumerable<ISomeInterface> _implementations;
    private readonly IEnumerable<ISomeInterface> _fromSampleNamespace;
    private readonly IEnumerable<(string name, ISomeInterface instance)> _implementationsWithNames;

    public DependentClass(INamedResolver<ISomeInterface> resolver)
    {
        _implementations = resolver.GetAll();
        _implementationsWithNames = resolver.GetAllWithNames();
        _fromSampleNamespace = resolver.GetAll(t => t.Namespace.StartsWith("Sample"));
	}
}

```