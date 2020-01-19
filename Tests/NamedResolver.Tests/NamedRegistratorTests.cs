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
        public void DisallowRegisterMultipleTimesWithSameName()
        {
            _namedRegistrator.Add("T1", typeof(T1));

            Assert.Throws<InvalidOperationException>(() => _namedRegistrator.Add("T1", typeof(T1)));
        }

        [TestCase(null, typeof(T1))]
        [TestCase("", typeof(T1))]
        public void AllowedNamesForDefaultType(string name, Type type)
        {
            Assert.DoesNotThrow(() => _namedRegistrator.Add(name, type));
        }

        [Test]
        public void AllowOnlyOneDefaultType()
        {
            _namedRegistrator.Add(null, typeof(T1));

            Assert.Throws<InvalidOperationException>(() => _namedRegistrator.Add(null, typeof(T1)));
        }

        [TestCase(null, typeof(T3))]
        [TestCase(null, typeof(DateTime))]
        [TestCase("1", typeof(DateTime))]
        public void AllowOnlyTypesThatImplementsInterface(string name, Type type)
        {
            Assert.Throws<InvalidOperationException>(() => _namedRegistrator.Add(name, type));
        }

        #endregion Тесты
    }
}