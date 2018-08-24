using System;
using System.Linq;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Autofac.Extras.UnitTestExtensions.UnitTest
{
    [TestFixture]
    public class ResolverTest
    {
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

        private IContainer CreateSutSimpleContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<Dog>();
            containerBuilder.RegisterType<Cat>();

            containerBuilder.RegisterType<WellFormedZoo>();
            containerBuilder.RegisterType<UnknownBirdZoo>();
            containerBuilder.RegisterType<FactoryDrivenZoo>();

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
