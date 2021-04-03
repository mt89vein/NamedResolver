using Microsoft.Extensions.DependencyInjection;
using NamedResolver.Abstractions;
using NamedResolver.Tests.TestClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NamedResolver.Tests
{
    [TestFixtureSource(nameof(EnumTestFixtureCases))]
    public class EnumedResolverTests
    {
        #region Поля

        private readonly INamedResolver<EnumForResolver, ITest> _namedResolver;
        private readonly IServiceProvider _serviceProvider;

        #endregion Поля

        #region Конструктор теста

        public static IEnumerable<TestFixtureData> EnumTestFixtureCases()
        {
            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<EnumForResolver, ITest>(ServiceLifetime.Singleton)
                    .Add<T1>(EnumForResolver.T1)
                    .Add(typeof(T1), EnumForResolver.T1_1)
                    .Add<T2>(EnumForResolver.T2)
                    .Add(typeof(T2), EnumForResolver.T2_1)
                    .Add(typeof(T2), sp => sp.GetRequiredService<T2>(), EnumForResolver.T2_1_Factory)
                    .Add<T2>(sp => sp.GetRequiredService<T2>(), EnumForResolver.T2_1_Generic_Factory)
                    .Add<T2>(sp => new T2(), EnumForResolver.T2_2_Generic_Factory)
                    .Add<T2>(EnumForResolver.TTT)
                    .Add<T1>(sp => sp.GetRequiredService<T1>());

                yield return new TestFixtureData("EnumFirstFixtureCase", services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<EnumForResolver, ITest>(ServiceLifetime.Singleton)
                    .TryAdd<T1>(EnumForResolver.T1) // +
                    .TryAdd(typeof(T1), EnumForResolver.T1_1) // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>(), EnumForResolver.T1_1) // -
                    .TryAdd<T2>(EnumForResolver.T2) // +
                    .TryAdd(typeof(T2), EnumForResolver.T2_1) // +
                    .TryAdd(typeof(T2), sp => sp.GetRequiredService<T2>(), EnumForResolver.T2_1_Factory) // +
                    .TryAdd<T2>(sp => sp.GetRequiredService<T2>(), EnumForResolver.T2_1_Generic_Factory) // +
                    .TryAdd<T2>(sp => new T2(), EnumForResolver.T2_2_Generic_Factory)
                    .TryAdd(typeof(T2), EnumForResolver.TTT) // +
                    .TryAdd(typeof(T1)) // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>()); // -

                yield return new TestFixtureData("EnumSecondFixtureCase", services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<EnumForResolver, ITest>(ServiceLifetime.Singleton)
                    .TryAdd<T1>(EnumForResolver.T1) // +
                    .TryAdd(typeof(T1), EnumForResolver.T1_1) // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>(), EnumForResolver.T1_1) // -
                    .TryAdd<T2>(EnumForResolver.T2) // +
                    .TryAdd(typeof(T2), EnumForResolver.T2_1) // +
                    .TryAdd(typeof(T2), sp => sp.GetRequiredService<T2>(), EnumForResolver.T2_1_Factory) // +
                    .TryAdd<T2>(sp => sp.GetRequiredService<T2>(), EnumForResolver.T2_1_Generic_Factory) // +
                    .TryAdd<T2>(sp => new T2(), EnumForResolver.T2_2_Generic_Factory) // +
                    .TryAdd(typeof(T2), EnumForResolver.TTT) // +
                    .Add(typeof(T1));

                yield return new TestFixtureData("EnumThirdFixtureCase", services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<EnumForResolver, ITest>(ServiceLifetime.Singleton)
                    .TryAdd<T1>(EnumForResolver.T1) // +
                    .TryAdd(typeof(T1), EnumForResolver.T1_1) // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>(), EnumForResolver.T1_1) // -
                    .TryAdd<T2>(EnumForResolver.T2) // +
                    .TryAdd(typeof(T2), EnumForResolver.T2_1) // +
                    .TryAdd(typeof(T2), sp => sp.GetRequiredService<T2>(), EnumForResolver.T2_1_Factory) // +
                    .TryAdd<T2>(sp => sp.GetRequiredService<T2>(), EnumForResolver.T2_1_Generic_Factory) // +
                    .TryAdd<T2>(sp => new T2(), EnumForResolver.T2_2_Generic_Factory) // +
                    .TryAdd(typeof(T2), EnumForResolver.TTT) // +
                    .TryAdd(typeof(T1), sp => sp.GetRequiredService<T1>()); // +

                yield return new TestFixtureData("EnumFourthFixtureCase", services.BuildServiceProvider());
            }
        }

        public EnumedResolverTests(string caseName, IServiceProvider sp)
        {
            _namedResolver = sp.GetRequiredService<INamedResolver<EnumForResolver, ITest>>();
            _serviceProvider = sp;
        }

        #endregion Конструктор теста

        #region Тесты

        [TestCase(EnumForResolver.T1, typeof(T1))]
        [TestCase(EnumForResolver.T1_1, typeof(T1))]
        [TestCase(EnumForResolver.T2, typeof(T2))]
        [TestCase(EnumForResolver.T2_1_Factory, typeof(T2))]
        [TestCase(EnumForResolver.T2_1_Generic_Factory, typeof(T2))]
        [TestCase(EnumForResolver.TTT, typeof(T2))]
        [TestCase(default(EnumForResolver), typeof(T1))]
        [TestCase(EnumForResolver.Default, typeof(T1))]
        public void CorrectlyResolveByName(EnumForResolver enumForResolver, Type type)
        {
            var fromGet = _namedResolver.Get(enumForResolver);
            var fromGet2 = _namedResolver.Get(enumForResolver);
            var itemFound = _namedResolver.TryGet(out var fromTryGet, enumForResolver);
            var fromIndexer = _namedResolver[enumForResolver];

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(fromGet);
                Assert.IsNotNull(fromTryGet);
                Assert.IsNotNull(fromIndexer);

                Assert.IsTrue(itemFound);

                Assert.IsInstanceOf(type, fromGet);
                Assert.IsInstanceOf(type, fromTryGet);
                Assert.IsInstanceOf(type, fromIndexer);

                Assert.AreSame(fromGet, fromGet2);
                Assert.AreSame(fromGet, fromTryGet);
                Assert.AreSame(fromIndexer, fromTryGet);
            });
        }

        [TestCase(EnumForResolver.T2_2_Generic_Factory, typeof(T2))]
        public void Registered_As_Factory_ReturnsNewInstanceEveryTime_If_Not_Proxied_Resolve_To_ServiceProvider(EnumForResolver enumForResolver, Type type)
        {
            var fromGet = _namedResolver.Get(enumForResolver);
            var fromGet2 = _namedResolver.Get(enumForResolver);
            var itemFound = _namedResolver.TryGet(out var fromTryGet, enumForResolver);
            var fromIndexer = _namedResolver[enumForResolver];

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(fromGet);
                Assert.IsNotNull(fromTryGet);
                Assert.IsNotNull(fromIndexer);

                Assert.IsTrue(itemFound);

                Assert.IsInstanceOf(type, fromGet);
                Assert.IsInstanceOf(type, fromTryGet);
                Assert.IsInstanceOf(type, fromIndexer);

                Assert.AreNotSame(fromGet, fromGet2);
                Assert.AreNotSame(fromGet, fromTryGet);
                Assert.AreNotSame(fromIndexer, fromTryGet);
            });
        }

        [TestCase((EnumForResolver)33)]
        [TestCase((EnumForResolver)44)]
        [TestCase((EnumForResolver)55)]
        public void DoesNotThrowIfNotFound(EnumForResolver enumForResolver)
        {
            Assert.DoesNotThrow(() =>
            {
                var fromGet = _namedResolver.Get(enumForResolver);
                var itemFound = _namedResolver.TryGet(out var fromTryGet, enumForResolver);
                var fromIndexer = _namedResolver[enumForResolver];

                Assert.IsNull(fromGet);
                Assert.IsNull(fromTryGet);
                Assert.IsNull(fromIndexer);
                Assert.IsFalse(itemFound);
            });
        }

        [TestCase((EnumForResolver)33)]
        [TestCase((EnumForResolver)44)]
        [TestCase((EnumForResolver)55)]
        public void ThrowIfNotFound(EnumForResolver enumForResolver)
        {
            Assert.Throws<InvalidOperationException>(() => _namedResolver.GetRequired(enumForResolver));
        }

        [Test]
        public void DefaultInstanceIncludesInGetAll()
        {
            var instances = _namedResolver.GetAll();

            Assert.AreEqual(9, instances.Count);
        }

        [Test]
        public void DefaultInstanceDoesNotIncludesInGetAllWithNames()
        {
            var instances = _namedResolver.GetAllWithNames();
            var (name, defaultInstance) = instances.FirstOrDefault(t => t.name == default);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(9, instances.Count);
                Assert.IsNotNull(defaultInstance);
                Assert.AreEqual(EnumForResolver.Default, name);
            });
        }

        [Test]
        public void GetAllWithPredicate()
        {
            var instances = _namedResolver.GetAll(t => t.Name.Contains("T1"));

            Assert.AreEqual(3, instances.Count);
        }

        [Test]
        public void GetAllWithNamesPredicate()
        {
            var instances = _namedResolver.GetAllWithNames((name, t) => t.Name.Contains("T1"));

            Assert.AreEqual(3, instances.Count);
        }

        [Test]
        public void DefaultImplementationExplicitlyResolvesCorrectly()
        {
            var dependentClass = _serviceProvider.GetRequiredService<DependentClass>();

            var defaultInstance = _namedResolver.Get();

            Assert.AreSame(defaultInstance, dependentClass.Test);
        }

        #endregion Тесты
    }
}