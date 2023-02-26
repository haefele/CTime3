using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Infrastructure;

public sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly IServiceProvider _provider;

    public TypeResolver(IServiceProvider provider)
    {
        Guard.IsNotNull(provider);

        this._provider = provider;
    }

    public object? Resolve(Type? type)
    {
        return type is null
            ? null
            : _provider.GetRequiredService(type);
    }

    public void Dispose()
    {
        if (_provider is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}