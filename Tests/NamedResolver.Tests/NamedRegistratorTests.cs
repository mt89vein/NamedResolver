using NamedResolver.Abstractions;
using NamedResolver.Tests.TestClasses;
using NUnit.Framework;
using System;

namespace NamedResolver.Tests
{
    [TestFixture(TestOf = typeof(INamedRegistrator<>))]
    public class NamedRegistratorTests
    {
        #region Поля

        private INamedRegistrator<ITest> _namedRegistrator;

        #endregion Поля

        #region Конструктор теста

        [SetUp]
        public void Setup()
        {
            _namedRegistrator = new NamedRegistrator<ITest>();
        }

        #endregion Конструктор теста

        #region Тесты

        [Test]
        public void AllowRegisterSameTypeMultipleTimes()
        {
            Assert.DoesNotThrow(() =>
            {
                _namedRegistrator.Add("T1", typeof(T2));
                _namedRegistrator.Add(null, typeof(T2));
                _namedRegistrator.Add("T2", typeof(T2));
                _namedRegistrator.Add("T3", typeof(T1));
                _namedRegistrator.Add("T4", typeof(T1));
            });
        }

        [Test]
        public void AllowTryAddSameTypeMultipleTimes()
        {
            Assert.Multiple(() =>
            {
                Assert.IsTrue(_namedRegistrator.TryAdd("T1", typeof(T2)));
                Assert.IsTrue(_namedRegistrator.TryAdd(null, typeof(T2)));
                Assert.IsTrue(_namedRegistrator.TryAdd("T2", typeof(T2)));
                Assert.IsTrue(_namedRegistrator.TryAdd("T3", typeof(T1)));
                Assert.IsTrue(_namedRegistrator.TryAdd("T4", typeof(T1)));
            });
        }

        [Test]
        public void DisallowRegisterMultipleTimesWithSameName()
        {
            _namedRegistrator.Add("Test", typeof(T1));

            Assert.Throws<InvalidOperationException>(() => _namedRegistrator.Add("Test", typeof(T2)));
        }

        [TestCase("Test")]
        [TestCase("T1")]
        [TestCase("T2")]
        public void TryAddReturnsFalseWhenRegisterMultipleTimesWithSameName(string name)
        {
            var expectedRegisteredType = typeof(T1);
            _namedRegistrator.Add(name, expectedRegisteredType);

            Assert.IsFalse(_namedRegistrator.TryAdd(name, typeof(T2)));

            var actualRegisteredType = ((IHasRegisteredTypeInfos<ITest>)_namedRegistrator).RegisteredTypes[name];

            Assert.AreEqual(expectedRegisteredType, actualRegisteredType);
        }

        [TestCase("")]
        [TestCase(null)]
        public void TryAddReturnsFalseWhenRegisterMultipleTimesForDefaultType(string name)
        {
            var expectedRegisteredType = typeof(T1);
            _namedRegistrator.Add(name, expectedRegisteredType);

            Assert.IsFalse(_namedRegistrator.TryAdd(name, typeof(T2)));

            var actualRegisteredType = ((IHasRegisteredTypeInfos<ITest>)_namedRegistrator).DefaultType;

            Assert.AreEqual(expectedRegisteredType, actualRegisteredType);
        }

        [TestCase(null, typeof(T1))]
        [TestCase("", typeof(T1))]
        public void AllowedNamesForAddDefaultType(string name, Type type)
        {
            Assert.DoesNotThrow(() => _namedRegistrator.Add(name, type));
        }

        [TestCase(null, typeof(T1))]
        [TestCase("", typeof(T1))]
        public void AllowedNamesForTryAddDefaultType(string name, Type type)
        {
            Assert.DoesNotThrow(() => _namedRegistrator.TryAdd(name, type));
        }

        [Test]
        public void AllowAddDefaultTypeOnlyOnce()
        {
            _namedRegistrator.Add(null, typeof(T1));

            Assert.Throws<InvalidOperationException>(() => _namedRegistrator.Add(null, typeof(T1)));
        }

        [Test]
        public void TryAddReturnsFalseIfDefaultTypeAlreadyAdded()
        {
            _namedRegistrator.Add(null, typeof(T1));

            Assert.IsFalse(_namedRegistrator.TryAdd(null, typeof(T2)));
        }

        [TestCase(null, typeof(T3))]
        [TestCase(null, typeof(DateTime))]
        [TestCase("1", typeof(DateTime))]
        public void AllowAddOnlyTypesThatImplementsInterface(string name, Type type)
        {
            Assert.Throws<InvalidOperationException>(() => _namedRegistrator.Add(name, type));
        }

        [TestCase(null, typeof(T3))]
        [TestCase(null, typeof(DateTime))]
        [TestCase("1", typeof(DateTime))]
        public void AllowTryAddOnlyTypesThatImplementsInterface(string name, Type type)
        {
            Assert.Throws<InvalidOperationException>(() => _namedRegistrator.TryAdd(name, type));
        }

        #endregion Тесты
    }
}