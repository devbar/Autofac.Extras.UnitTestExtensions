using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Extras.UnitTestExtensions.Resolver;
using JetBrains.Annotations;

namespace Autofac.Extras.UnitTestExtensions
{
    public static class ContainerExtension
    {
        [CanBeNull]
        private static IContainer _container;

        public static IEnumerable<Type> GetUnresolvableTypes([NotNull] this IContainer sutContainer)
        {
            if(sutContainer == null)
                throw new ArgumentNullException(nameof(sutContainer));

            if (_container == null)
                _container = GetModuleContainer();
            
            foreach(var resolver in _container.Resolve<IEnumerable<IResolver>>())
            foreach (var type in resolver.GetUnresolvableTypes(sutContainer))
                yield return type;
        }
        
        [NotNull]
        private static IContainer GetModuleContainer()
        {
            var currentAssembly = Assembly.GetAssembly(typeof(ContainerExtension));
            var containerBuilder = new ContainerBuilder();

            containerBuilder
                .RegisterAssemblyTypes(currentAssembly)
                .Where(t => t
                    .GetInterfaces()
                    .Contains(typeof(IResolver)))
                .As<IResolver>();

            return containerBuilder.Build();
        }

    }
}
