using System.Reflection;

namespace Bruno57.Domain.Factory.Handlers;

public interface IFactoryMethodHandler
{
    public MethodInfo? GetFactoryMethod(Type type);
}
