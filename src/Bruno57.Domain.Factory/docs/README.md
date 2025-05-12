# Bruno57.Domain.Factory

## 💡 Why Bruno57.Domain.Factory?

In the realm of Domain-Driven Design, the construction of intricate object structures—particularly aggregates—shouldn't be the responsibility of the domain entities themselves.
should not be the responsibility of the entities themselves. According to DDD principles:

> “Complex object creation is a responsibility of the domain layer, yet that task does not belong to the objects that express the model…”

Bruno57.Domain.Factory provides a solution by:

* Orchestrating the creation of entire aggregates as a unified whole, thereby upholding their inherent rules.
* Presenting an interface that mirrors the client's intentions while abstracting away the intricacies of instantiation.
* Simplifying the management of both optional and mandatory characteristics when bringing an entity into existence.

## How It Works
At its core, Bruno57.Domain.Factory offers a versatile interface through Bruno57.Domain.Factory.Abstractions:
```csharp
TResponse? CreateEntityObject(TRequest request, [Optional] Action<DomainFactoryOption> action);
```
* TRequest: A simple request object (usually a command or DTO).
* TResponse: A domain entity that inherits from EntityBase.
* The optional DomainFactoryOption lets you fine-tune how the entity is created — such as ignoring or injecting specific properties.

Under the hood, it uses reflection to copy values from the request object to the target domain entity, 
respecting the customisation provided via the options. It is done by scanning an input request object 
and matching its properties to those of the target domain type.

Internally, it employs reflection to transfer values from the input object to the target domain entity, respecting any customizations defined through the options. This is achieved by examining the input object and matching its properties with those of the intended domain type.

Crucially, all of this happens without exposing the concrete type or internal structure of the domain object to the consumer, aligning perfectly with Domain-Driven Design principles.

##  Performance Considerations
While this library leverages .NET reflection for the dynamic construction of domain entities, it's engineered with efficiency in mind.

Underneath, it utilizes a static, thread-safe cache to eliminate redundant reflection operations

```csharp
protected static ConcurrentDictionary<string, MethodInfo> CachedMethodInfoCollection { get; set; } = new();
```

This caching mechanism ensures that each method or constructor lookup occurs only once per unique key and is reused for subsequent assembler calls:

```csharp
var method = CachedMethodInfoCollection.TryGetValue(cacheKey, out var result)
    ? result
    : GetMethod();

if (method is null)
{
    return null;
}

CachedMethodInfoCollection[cacheKey] = method;
```

> This design minimizes any performance overhead associated with reflection, ensuring the assembler remains performant and scalable.

## Getting Started
### Basic Implementation

First, register the Domain Assembler service within your application's setup:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDomainFactory();
```

Then, a simple usage would be like this:

```csharp
public class CreateSomethingHandler : ICommandHandler<CreateSomethingCommand, SomeResult<int>>
{
    private readonly IDomainFactory<CreateSomethingCommand, EntityClass> _domainFactory;

    public CreateSomethingHandler(IDomainFactory<CreateSomethingCommand, EntityClass> domainFactory)
    {
        _domainFactory = domainFactory.CheckForNull();
    }

    public async Task<Result<int>> Handle(CreateSomethingCommand request, CancellationToken cancellationToken)
    {
        var myEntity = _domainFactory.CreateEntityObject(request);

        // Use your domain entity...
    }
}
```
### Advanced Implementation with Customization

```csharp
public class CreateSomethingHandler : ICommandHandler<CreateSomethingCommand, SomeResult<int>>
{
    private readonly IDomainFactory<CreateSomethingCommand, EntityClass> _domainFactory;

    public CreateSomethingHandler(IDomainFactory<CreateSomethingCommand, EntityClass> domainFactory)
    {
        _domainFactory = domainFactory.CheckForNull();
    }

    public async Task<Result<int>> Handle(CreateSomethingCommand request, CancellationToken cancellationToken)
    {
        var propertyValue = "ExampleValueForNewProperty";

        var domainFactoryResponse = _domainFactory.CreateEntityObject(request, option =>
        {
            option.IgnoreProperties([
                nameof(request.SomePropertyOne),
                nameof(request.SomePropertyTwo)
            ]);

            option.AddProperties(new Dictionary<string, object>
            {
                { "SomePropertyThree", propertyValue },
                { "SomePropertyFour", 2025 }
            });
        });

        // Use your customised domain entity...
    }
}
```

##  Complementary Packages
### [Bruno57.Domain.Foundations](https://www.nuget.org/packages/Bruno57.Domain.Foundations)
Provides core DDD primitives like EntityBase, as well as helpful attributes like [AggregateRoot].

> This is a transitive dependency of Foundations.Domain.Factory, so you don’t need to install it separately.

## When This Might Not Be The Right Choice
The rationale behind this package is to assist developers working on substantial DDD projects with numerous entities and potentially many factories, by automating the object creation process. This eliminates the need for manual factory instantiation; the package handles it for you. You simply specify the desired type and the required property values.

For smaller DDD initiatives or other architectural styles where a limited number of factories are needed, Ghanavats.Domain.Assembler might introduce unnecessary complexity.

Avoid adding complexity without clear benefit.

Keep your solutions straightforward.

## 📜 License
MIT License
