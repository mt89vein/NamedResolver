using Microsoft.Extensions.DependencyInjection;
using NamedResolver.Abstractions;
using NamedResolver.Tests.TestClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NamedResolver.Tests
{
    [TestFixtureSource(nameof(TestFixtureCases))]
    public class NamedResolverTests
    {
        #region Поля

        private readonly INamedResolver<ITest> _namedResolver;
        private readonly IServiceProvider _serviceProvider;

        #endregion Поля

        #region Конструктор теста

        public static IEnumerable<TestFixtureData> TestFixtureCases()
        {
            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<ITest>(ServiceLifetime.Singleton)
                    .Add<T1>("T1")
                    .Add(typeof(T1), "T1-1")
                    .Add<T2>("T2")
                    .Add(typeof(T2), "T2-1")
                    .Add(typeof(T2), sp => sp.GetRequiredService<T2>(), "T2-1-Factory")
                    .Add<T2>(sp => sp.GetRequiredService<T2>(), "T2-1-Generic-Factory")
                    .Add<T2>("TTT")
                    .Add<T1>(sp => sp.GetRequiredService<T1>());

                yield return new TestFixtureData("FirstFixtureCase", services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<ITest>(ServiceLifetime.Singleton)
                    .TryAdd<T1>("T1") // +
                    .TryAdd(typeof(T1), "T1-1") // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>(), "T1-1") // -
                    .TryAdd<T2>("T2") // +
                    .TryAdd(typeof(T2), "T2-1") // +
                    .TryAdd(typeof(T2), sp => sp.GetRequiredService<T2>(), "T2-1-Factory") // +
                    .TryAdd<T2>(sp => sp.GetRequiredService<T2>(), "T2-1-Generic-Factory") // +
                    .TryAdd(typeof(T2), "TTT") // +
                    .TryAdd(typeof(T1)) // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>()); // -

                yield return new TestFixtureData("SecondFixtureCase", services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<ITest>(ServiceLifetime.Singleton)
                    .TryAdd<T1>("T1") // +
                    .TryAdd(typeof(T1), "T1-1") // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>(), "T1-1") // -
                    .TryAdd<T2>("T2") // +
                    .TryAdd(typeof(T2), "T2-1") // +
                    .TryAdd(typeof(T2), sp => sp.GetRequiredService<T2>(), "T2-1-Factory") // +
                    .TryAdd<T2>(sp => sp.GetRequiredService<T2>(), "T2-1-Generic-Factory") // +
                    .TryAdd(typeof(T2), "TTT") // +
                    .Add(typeof(T1)); // +

                yield return new TestFixtureData("ThirdFixtureCase", services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<ITest>(ServiceLifetime.Singleton)
                    .TryAdd<T1>("T1") // +
                    .TryAdd(typeof(T1), "T1-1") // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>(), "T1-1") // -
                    .TryAdd<T2>("T2") // +
                    .TryAdd(typeof(T2), "T2-1") // +
                    .TryAdd(typeof(T2), sp => sp.GetRequiredService<T2>(), "T2-1-Factory") // +
                    .TryAdd<T2>(sp => sp.GetRequiredService<T2>(), "T2-1-Generic-Factory") // +
                    .TryAdd(typeof(T2), "TTT") // +
                    .TryAdd(typeof(T1), sp => sp.GetRequiredService<T1>()); // +

                yield return new TestFixtureData("FourthFixtureCase", services.BuildServiceProvider());
            }
        }

        public NamedResolverTests(string caseName, IServiceProvider sp)
        {
            _namedResolver = sp.GetRequiredService<INamedResolver<ITest>>();
            _serviceProvider = sp;
        }

        #endregion Конструктор теста

        #region Тесты

        [TestCase("T1", typeof(T1))]
        [TestCase("T1-1", typeof(T1))]
        [TestCase("T2", typeof(T2))]
        [TestCase("T2-1-Factory", typeof(T2))]
        [TestCase("T2-1-Generic-Factory", typeof(T2))]
        [TestCase("TTT", typeof(T2))]
        [TestCase("", typeof(T1))]
        [TestCase(null, typeof(T1))]
        public void CorrectlyResolveByName(string name, Type type)
        {
            var fromGet = _namedResolver.Get(name);
            var itemFound = _namedResolver.TryGet(out var fromTryGet, name);
            var fromIndexer = _namedResolver[name];

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(fromGet);
                Assert.IsNotNull(fromTryGet);
                Assert.IsNotNull(fromIndexer);

                Assert.IsTrue(itemFound);

                Assert.AreEqual(type, fromGet.GetType());
                Assert.AreEqual(type, fromTryGet.GetType());
                Assert.AreEqual(type, fromIndexer.GetType());

                Assert.AreSame(fromGet, fromTryGet);
                Assert.AreSame(fromIndexer, fromTryGet);
            });
        }

        [TestCase("T3")]
        [TestCase("T4")]
        [TestCase("T5")]
        public void DoesNotThrowIfNotFound(string name)
        {
            Assert.DoesNotThrow(() =>
            {
                var fromGet = _namedResolver.Get(name);
                var itemFound = _namedResolver.TryGet(out var fromTryGet, name);
                var fromIndexer = _namedResolver[name];

                Assert.IsNull(fromGet);
                Assert.IsNull(fromTryGet);
                Assert.IsNull(fromIndexer);
                Assert.IsFalse(itemFound);
            });
        }

        [TestCase("T3")]
        [TestCase("T4")]
        [TestCase("T5")]
        public void ThrowIfNotFound(string name)
        {
            Assert.Throws<InvalidOperationException>(() => _namedResolver.GetRequired(name));
        }

        [Test]
        public void DefaultInstanceIncludesInGetAll()
        {
            var instances = _namedResolver.GetAll();

            Assert.AreEqual(8, instances.Count);
        }

        [Test]
        public void DefaultInstanceDoesNotIncludesInGetAllWithNames()
        {
            var instances = _namedResolver.GetAllWithNames();
            var (name, defaultInstance) = instances.FirstOrDefault(t => string.IsNullOrEmpty(t.name));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(8, instances.Count);
                Assert.IsNotNull(defaultInstance);
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
