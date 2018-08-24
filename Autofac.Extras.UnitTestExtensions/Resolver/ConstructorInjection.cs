using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac.Core.Lifetime;
using Autofac.Extras.UnitTestExtensions.Resolver;

namespace Autofac.Extras.UnitTestExtensions
{
    public class ConstructorInjection : IResolver
    {
        public IEnumerable<Type> GetUnresolvableTypes(IContainer container)
            => GetTypesRegisteredInContainer(container).SelectMany(type => GetUnresolvableTypes(container, type));

        private IEnumerable<Type> GetUnresolvableTypes(IComponentContext container, Type type)
        {
            foreach (var c in GetPublicConstructors(type))
            {
                if(IsDirectlyResolvable(container, c))
                    yield break;

                if (IsFactoryRegisteredFor(container, type, c))
                    yield break;
            }

            yield return type;
        }

        private static IEnumerable<ConstructorInfo> GetAllPublicConstructors(IComponentContext container)
            => GetTypesRegisteredInContainer(container).SelectMany(GetPublicConstructors);

        private static IEnumerable<ConstructorInfo> GetPublicConstructors(Type type)
            => type.GetConstructors().Where(ctor => ctor.IsPublic);

        /// <summary>
        /// Checks for direct resolving without any "special" features of Autofac like factories, lazy etc.
        /// </summary>
        private static bool IsDirectlyResolvable(IComponentContext container, MethodBase ctrInfo)
            => ctrInfo.GetParameters().Select(p => container.IsRegistered(p.ParameterType)).All(registered => registered);

        /// <summary>
        /// Checks if there is a factory creating this type. In this case unresolved parameters are allowed.
        /// </summary>
        private static bool IsFactoryRegisteredFor(IComponentContext container, Type type, MethodBase info)
        {
            var argsInfo = info.GetParameters().Select(p => p.ParameterType).ToArray();
            var funcType = GetFuncType(argsInfo.Length);

            foreach (var ctor in GetAllPublicConstructors(container))
            {
                var factoryFunc =
                    ctor
                        .GetParameters()
                        .Where(p => p.ParameterType.IsGenericType)
                        .Where(p => p.ParameterType.GetGenericTypeDefinition() == funcType)
                        .Select(p => p.ParameterType);

                foreach (var factory in factoryFunc)
                {
                    var argsGeneric = factory.GetGenericArguments();
                    var match = true;

                    for (var i = 0; i < (argsGeneric.Length - 1); i++)
                    {
                        if (argsGeneric[i] == argsInfo[i])
                            continue;

                        match = false;
                        break;
                    }

                    if (!match)
                        continue;

                    if (argsGeneric[argsGeneric.Length - 1] == type)
                        return true;

                    if (type.GetInterfaces().Contains(argsGeneric[argsGeneric.Length - 1]))
                        return true;

                }
            }

            return false;
        }

        /// <summary>
        /// Returns a list of all interfaces registered by this container.
        /// Interfaces and autofac objects are skipped.
        /// </summary>
        private static IEnumerable<Type> GetTypesRegisteredInContainer(IComponentContext container)
            => container
                   .ComponentRegistry
                   .Registrations
                   .Select(x => x.Activator.LimitType)
                   .Where(t => t != typeof(LifetimeScope) && !t.IsInterface);

        /// <summary>
        /// Simple dirty method to get the right type for a func with generic args.
        /// </summary>
        private static Type GetFuncType(int argCount)
        {
            switch (argCount)
            {
                case 0:
                    return typeof(Func<>);

                case 1:
                    return typeof(Func<,>);

                case 2:
                    return typeof(Func<,,>);

                case 3:
                    return typeof(Func<,,,>);

                case 4:
                    return typeof(Func<,,,,>);

                case 5:
                    return typeof(Func<,,,,,>);

                case 6:
                    return typeof(Func<,,,,,,>);

                case 7:
                    return typeof(Func<,,,,,,,>);

                case 8:
                    return typeof(Func<,,,,,,,,>);

                case 9:
                    return typeof(Func<,,,,,,,,,>);

                case 10:
                    return typeof(Func<,,,,,,,,,,>);

                case 11:
                    return typeof(Func<,,,,,,,,,,,>);

                case 12:
                    return typeof(Func<,,,,,,,,,,,,>);

                case 13:
                    return typeof(Func<,,,,,,,,,,,,,>);

                case 14:
                    return typeof(Func<,,,,,,,,,,,,,,>);

                case 15:
                    return typeof(Func<,,,,,,,,,,,,,,,>);

                case 16:
                    return typeof(Func<,,,,,,,,,,,,,,,,>);

                default:
                    throw new ArgumentException("To much arguments, think about your design");
            }

        }
    }
}
