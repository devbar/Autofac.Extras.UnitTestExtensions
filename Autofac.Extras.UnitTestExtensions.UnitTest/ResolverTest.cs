using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Features.OwnedInstances;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Autofac.Extras.UnitTestExtensions.UnitTest
{
    [TestFixture]
    public class ResolverTest
    {
        #region Base Classes

        public interface IAnimal { string Name { get; } }

        public class Dog : IAnimal
        {
            public Dog()
            {
            }

            public Dog(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }

        public class Cat : IAnimal
        {
            public Cat()
            {
            }

            public Cat(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }

        public class Bird : IAnimal
        {
            public Bird()
            {
            }

            public Bird(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }

        #endregion Base Classes

        public class WellFormedZoo
        {
            public WellFormedZoo(Dog dog, Cat cat) {}
        }

        public class UnknownBirdZoo
        {
            public UnknownBirdZoo(Dog dog, Cat cat, Bird bird) { }
        }

        public class FactoryDrivenZoo
        {
            public FactoryDrivenZoo(Dog dog, Func<string, Cat> catFactory)
            {
            }
        }

        public class LazyDogZoo
        {
            public LazyDogZoo(Lazy<Dog> lazyDog)
            {

            }
        }

        public class CatListZoo
        {
            public CatListZoo(IEnumerable<Cat> lazyDog)
            {

            }
        }

        public class OwnedCatZoo
        {
            public OwnedCatZoo(Owned<Cat> cat)
            {

            }
        }

        private static IContainer CreateSutSimpleContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<Dog>();
            containerBuilder.RegisterType<Cat>();

            containerBuilder.RegisterType<WellFormedZoo>();
            containerBuilder.RegisterType<UnknownBirdZoo>();
            containerBuilder.RegisterType<FactoryDrivenZoo>();
            containerBuilder.RegisterType<LazyDogZoo>();
            containerBuilder.RegisterType<CatListZoo>();
            containerBuilder.RegisterType<OwnedCatZoo>();

            return containerBuilder.Build();
        }

        [Test]
        public void ShouldResolveCtrInjection()
        {
            var container = CreateSutSimpleContainer();
            var types = container.GetUnresolvableTypes().ToArray();

            Assert.AreEqual(1, types.Count(), "UnresolvedTypes: " + string.Join(", ", types.Select(t => t.ToString())));
            Assert.AreEqual(nameof(UnknownBirdZoo), types[0].Name);
        }
    }
}
