using Microsoft.Extensions.DependencyInjection;
using NamedResolver.Abstractions;
using NamedResolver.Tests.TestClasses;
using NUnit.Framework;
using System;

namespace NamedResolver.Tests
{
    [TestFixture]
    public class NamedDescriptorTests
    {
        [Test]
        public void DefaultsAreEquals()
        {
            var d1 = new NamedDescriptor<string, ITest>();
            var d2 = new NamedDescriptor<string, ITest>(default);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(d1, d2);
                Assert.IsTrue(d1.Equals((object)d2), "Must be equal with boxing");
                Assert.IsTrue(d1 == d2);
                Assert.IsTrue(d1.GetHashCode() == d2.GetHashCode());
            });
        }

        [Test]
        public void EqualsWithSameName()
        {
            var d1 = new NamedDescriptor<string, ITest>("T1");
            var d2 = new NamedDescriptor<string, ITest>("T1");

            Assert.Multiple(() =>
            {
                Assert.AreEqual(d1, d2);
                Assert.IsTrue(d1.Equals((object)d2), "Must be equal with boxing");
                Assert.IsTrue(d1 == d2);
                Assert.IsTrue(d1.GetHashCode() == d2.GetHashCode());
            });
        }

        [Test]
        public void NotEqualsWithDifferentNames()
        {
            var d1 = new NamedDescriptor<string, ITest>("T1");
            var d2 = new NamedDescriptor<string, ITest>("T2");

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(d1, d2);
                Assert.IsFalse(d1.Equals((object)d2), "Must not be equal with boxing");
                Assert.IsTrue(d1 != d2);
                Assert.IsTrue(d1.GetHashCode() != d2.GetHashCode());
            });
        }

        [Test]
        public void NotEqualsWithDifferentType()
        {
            var d1 = new NamedDescriptor<string, ITest>("T1");
            var d2 = new Guid();

            Assert.Multiple(() =>
            {
                Assert.AreNotEqual(d1, d2);
                Assert.IsFalse(d1.Equals((object)d2), "Must not be equal with boxing");
                Assert.IsTrue(d1.GetHashCode() != d2.GetHashCode());
            });
        }

        [Test]
        public void EqualityComparerWorks()
        {
            var d1 = new NamedDescriptor<string, ITest>("T1", StringComparer.OrdinalIgnoreCase);
            var d2 = new NamedDescriptor<string, ITest>("t1", StringComparer.OrdinalIgnoreCase);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(d1, d2);
                Assert.IsTrue(d1.Equals((object)d2), "Must be equal with boxing");
                Assert.IsTrue(d1 == d2);
                Assert.IsTrue(d1.GetHashCode() == d2.GetHashCode());
            });
        }

        [Test]
        public void ResolveTest()
        {
            var services = new ServiceCollection();
            services.AddNamed<string, ITest>(ServiceLifetime.Singleton)
                .Add<T1>(_ => null); // error!

            var sp = services.BuildServiceProvider();
            var resolver = sp.GetRequiredService<INamedResolver<string, ITest>>();

            Assert.Throws<InvalidOperationException>(() => resolver.Get());
        }

        [Test]
        public void PredicateFilterTest()
        {
            var services = new ServiceCollection();
            services.AddNamed<string, ITest>(ServiceLifetime.Singleton)
                .Add<T1>()
                .Add<T1>(_ => new T1(), "T1")
                .Add<T2>(_ => new T2(), "T2");

            var sp = services.BuildServiceProvider();
            var resolver = sp.GetRequiredService<INamedResolver<string, ITest>>();

            Assert.AreEqual(2, resolver.GetAll(p => p == typeof(T1)).Count);
            Assert.AreEqual(2, resolver.GetAllWithNames((d, p) => p == typeof(T1)).Count);
            Assert.AreEqual(1, resolver.GetAll(p => p == typeof(T2)).Count);
            Assert.AreEqual(1, resolver.GetAllWithNames((d, p) => p == typeof(T2)).Count);
        }
    }
}
