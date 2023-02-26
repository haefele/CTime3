using System;
using CommunityToolkit.Diagnostics;
using CTime3.Core;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace CTime3.Apps.CommandLine.Infrastructure
{
    public sealed class TypeRegistrar : ITypeRegistrar
    {
        private readonly IServiceCollection _builder;

        public TypeRegistrar(IServiceCollection builder)
        {
            Guard.IsNotNull(builder);

            this._builder = builder;
        }

        public ITypeResolver Build()
        {
            return new TypeResolver(_builder.BuildServiceProvider());
        }

        public void Register(Type service, Type implementation)
        {
            Guard.IsNotNull(service);
            Guard.IsNotNull(implementation);

            this._builder.AddSingleton(service, implementation);
        }

        public void RegisterInstance(Type service, object implementation)
        {
            Guard.IsNotNull(service);
            Guard.IsNotNull(implementation);

            this._builder.AddSingleton(service, implementation);
        }

        public void RegisterLazy(Type service, Func<object> func)
        {
            Guard.IsNotNull(service);
            Guard.IsNotNull(func);

            this._builder.AddSingleton(service, (_) => func());
        }
    }
}
