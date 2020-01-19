using Microsoft.Extensions.DependencyInjection;
using NamedResolver.Abstractions;
using NamedResolver.Extensions;
using NamedResolver.Tests.TestClasses;
using NUnit.Framework;
using System;
using System.Linq;

namespace NamedResolver.Tests
{
    public class NamedResolverTests
    {
        #region Поля

        private readonly INamedResolver<ITest> _namedResolver;
        private readonly IServiceProvider _serviceProvider;

        #endregion Поля

        #region Конструктор теста

        public NamedResolverTests()
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

            var sp = services.BuildServiceProvider();
            var registrator = sp.GetRequiredService<INamedRegistrator<ITest>>();

            _namedResolver = new NamedResolver<ITest>(sp, registrator);
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
        public void DefaultInstanceDoesNotIncludesInGetAll()
        {
            var instances = _namedResolver.GetAll();

            Assert.AreEqual(7, instances.Count);
        }

        [Test]
        public void DefaultInstanceDoesNotIncludesInGetAllWithNames()
        {
            var instances = _namedResolver.GetAllWithNames();
            var (name, defaultInstance) = instances.FirstOrDefault(t => string.IsNullOrEmpty(t.name));

            Assert.Multiple(() =>
            {
                Assert.AreEqual(7, instances.Count);
                Assert.IsNull(defaultInstance);
            });
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
