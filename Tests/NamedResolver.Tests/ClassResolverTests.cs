using Microsoft.Extensions.DependencyInjection;
using NamedResolver.Abstractions;
using NamedResolver.Tests.TestClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using static NamedResolver.Tests.TestClasses.CustomClassForResolver;

namespace NamedResolver.Tests
{
    [TestFixtureSource(nameof(ClassTestFixtureCases))]
    public class ClassResolverTests
    {
        #region Поля

        private readonly INamedResolver<CustomClassForResolver, ITest> _namedResolver;
        private readonly IServiceProvider _serviceProvider;

        #endregion Поля

        #region Конструктор теста

        public static IEnumerable<TestFixtureData> ClassTestFixtureCases()
        {
            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<CustomClassForResolver, ITest>(ServiceLifetime.Singleton, CustomComparer)
                    .Add<T1>(CreateWith("T1"))
                    .Add(typeof(T1), CreateWith("T1-1"))
                    .Add<T2>(CreateWith("T2"))
                    .Add(typeof(T2), CreateWith("T2-1"))
                    .Add(typeof(T2), sp => sp.GetRequiredService<T2>(), CreateWith("T2-1-Factory"))
                    .Add<T2>(sp => sp.GetRequiredService<T2>(), CreateWith("T2-1-Generic-Factory"))
                    .Add<T2>(CreateWith("TTT"))
                    .Add<T1>(sp => sp.GetRequiredService<T1>());

                yield return new TestFixtureData("ClassFirstFixtureCase", services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<CustomClassForResolver, ITest>(ServiceLifetime.Singleton, CustomComparer)
                    .TryAdd<T1>(CreateWith("T1")) // +
                    .TryAdd(typeof(T1), CreateWith("T1-1")) // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>(), CreateWith("T1-1")) // -
                    .TryAdd<T2>(CreateWith("T2")) // +
                    .TryAdd(typeof(T2), CreateWith("T2-1")) // +
                    .TryAdd(typeof(T2), sp => sp.GetRequiredService<T2>(), CreateWith("T2-1-Factory")) // +
                    .TryAdd<T2>(sp => sp.GetRequiredService<T2>(), CreateWith("T2-1-Generic-Factory")) // +
                    .TryAdd(typeof(T2), CreateWith("TTT")) // +
                    .TryAdd(typeof(T1)) // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>()); // -

                yield return new TestFixtureData("ClassSecondFixtureCase", services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<CustomClassForResolver, ITest>(ServiceLifetime.Singleton, CustomComparer)
                    .TryAdd<T1>(CreateWith("T1")) // +
                    .TryAdd(typeof(T1), CreateWith("T1-1")) // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>(), CreateWith("T1-1")) // -
                    .TryAdd<T2>(CreateWith("T2")) // +
                    .TryAdd(typeof(T2), CreateWith("T2-1")) // +
                    .TryAdd(typeof(T2), sp => sp.GetRequiredService<T2>(), CreateWith("T2-1-Factory")) // +
                    .TryAdd<T2>(sp => sp.GetRequiredService<T2>(), CreateWith("T2-1-Generic-Factory")) // +
                    .TryAdd(typeof(T2), CreateWith("TTT")) // +
                    .Add(typeof(T1)); // +

                yield return new TestFixtureData("ClassThirdFixtureCase", services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<DependentClass>();
                services.AddNamed<CustomClassForResolver, ITest>(ServiceLifetime.Singleton, CustomComparer)
                    .TryAdd<T1>(CreateWith("T1")) // +
                    .TryAdd(typeof(T1), CreateWith("T1-1")) // +
                    .TryAdd<T1>(sp => sp.GetRequiredService<T1>(), CreateWith("T1-1")) // -
                    .TryAdd<T2>(CreateWith("T2")) // +
                    .TryAdd(typeof(T2), CreateWith("T2-1")) // +
                    .TryAdd(typeof(T2), sp => sp.GetRequiredService<T2>(), CreateWith("T2-1-Factory")) // +
                    .TryAdd<T2>(sp => sp.GetRequiredService<T2>(), CreateWith("T2-1-Generic-Factory")) // +
                    .TryAdd(typeof(T2), CreateWith("TTT"))// +
                    .TryAdd(typeof(T1), sp => sp.GetRequiredService<T1>()); // +

                yield return new TestFixtureData("ClassFourthFixtureCase", services.BuildServiceProvider());
            }
        }

        public ClassResolverTests(string caseName, IServiceProvider sp)
        {
            _namedResolver = sp.GetRequiredService<INamedResolver<CustomClassForResolver, ITest>>();
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
        [TestCase(default(string), typeof(T1))]
        [TestCase(null, typeof(T1))]
        public void CorrectlyResolveByName(string value, Type type)
        {
            var customClassForResolver = CreateWith(value);
            var fromGet = _namedResolver.Get(customClassForResolver);
            var itemFound = _namedResolver.TryGet(out var fromTryGet, customClassForResolver);
            var fromIndexer = _namedResolver[customClassForResolver];

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
        public void DoesNotThrowIfNotFound(string value)
        {
            var customClassForResolver = CreateWith(value);
            Assert.DoesNotThrow(() =>
            {
                var fromGet = _namedResolver.Get(customClassForResolver);
                var itemFound = _namedResolver.TryGet(out var fromTryGet, customClassForResolver);
                var fromIndexer = _namedResolver[customClassForResolver];

                Assert.IsNull(fromGet);
                Assert.IsNull(fromTryGet);
                Assert.IsNull(fromIndexer);
                Assert.IsFalse(itemFound);
            });
        }

        [TestCase("T3")]
        [TestCase("T4")]
        [TestCase("T5")]
        public void ThrowIfNotFound(string value)
        {
            var customClassForResolver = CreateWith(value);

            Assert.Throws<InvalidOperationException>(() => _namedResolver.GetRequired(customClassForResolver));
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
            var (name, defaultInstance) = instances.FirstOrDefault(t => t.name == default);

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