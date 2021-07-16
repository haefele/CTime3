using System;
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
            Guard.NotNull(builder, nameof(builder));
        
            this._builder = builder;
        }

        public ITypeResolver Build()
        {
            return new TypeResolver(_builder.BuildServiceProvider());
        }

        public void Register(Type service, Type implementation)
        {
            Guard.NotNull(service, nameof(service));
            Guard.NotNull(implementation, nameof(implementation));
            
            this._builder.AddSingleton(service, implementation);
        }

        public void RegisterInstance(Type service, object implementation)
        {
            Guard.NotNull(service, nameof(service));
            Guard.NotNull(implementation, nameof(implementation));
            
            this._builder.AddSingleton(service, implementation);
        }

        public void RegisterLazy(Type service, Func<object> func)
        {
            Guard.NotNull(service, nameof(service));
            Guard.NotNull(func, nameof(func));

            this._builder.AddSingleton(service, (provider) => func());
        }
    }
}