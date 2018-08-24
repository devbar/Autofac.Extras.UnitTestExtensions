# Autofac.Extras.UnitTestExtensions

Autofac is a game-changing framework for depedency injection. But not registered depedencies in constructors made me angry. Time to invent a framework to integrate registration validation in unit testing.

```
[Test]
public void ShouldResolveCtrInjection()
{
    var container = CreateSutSimpleContainer();
    var types = container.GetUnresolvableTypes().ToArray();

    Assert.AreEqual(1, types.Count(), "UnresolvedTypes: " + string.Join(", ", types.Select(t => t.ToString())));
    Assert.AreEqual(nameof(UnknownBirdZoo), types[0].Name);
}
```

DONE:
* Checking Constructors for registered dependencies
* Checking Constructors for registered factories (i.g. Func<myParamter,myRegistration> .. )

TODO:
* Checking Lazy, OwnedBy, IEnumerable
* Checking Property Injection
