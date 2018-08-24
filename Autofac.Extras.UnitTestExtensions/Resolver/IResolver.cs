using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Autofac.Extras.UnitTestExtensions.Resolver
{
    public interface IResolver
    {
        IEnumerable<Type> GetUnresolvableTypes([NotNull] IContainer container);
    }
}
