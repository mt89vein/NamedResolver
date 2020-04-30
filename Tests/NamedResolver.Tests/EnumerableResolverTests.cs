using Microsoft.Extensions.DependencyInjection;
using NamedResolver.Tests.TestClasses;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NamedResolver.Tests
{
    public class EnumerableResolverTests
    {
        #region Вспомогательные классы

        private class ClassWithEnumerable
        {
            public IEnumerable<ITest> Items { get; set; }

            public ClassWithEnumerable(IEnumerable<ITest> items)
            {
                Items = items;
            }
        }

        private class ClassWithIReadOnlyList
        {
            public IReadOnlyList<ITest> Items { get; set; }

            public ClassWithIReadOnlyList(IReadOnlyList<ITest> items)
            {
                Items = items;
            }
        }

        #endregion Вспомогательные классы

        #region Тесты

        [TestCaseSource(nameof(EnumerableTestCaseSource))]
        public void ClassWithIEnumerableResolvesWithAllRegistered(
            string testCaseNum,
            int expectedCount,
            IServiceProvider sp
        )
        {
            var one = sp.GetRequiredService<ClassWithEnumerable>();
            var two = sp.GetRequiredService<IEnumerable<ITest>>();
            var third = sp.GetServices<ITest>();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedCount, one.Items.Count(), testCaseNum);
                Assert.AreSame(one.Items, two, testCaseNum);
                Assert.AreSame(third, two, testCaseNum);
            });
        }

        [TestCaseSource(nameof(ReadOnlyListTestCaseSource))]
        public void ClassWithIReadOnlyListResolvesWithAllRegistered(
            string testCaseNum,
            int expectedCount,
            IServiceProvider sp
        )
        {
            var one = sp.GetRequiredService<ClassWithIReadOnlyList>();
            var two = sp.GetRequiredService<IReadOnlyList<ITest>>();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(expectedCount, one.Items.Count, testCaseNum);
                Assert.AreSame(one.Items, two, testCaseNum);
            });
        }

        #endregion Тесты

        #region Тест кейсы

        /// <summary>
        /// Тест кейсы для <seealso cref="ClassWithIReadOnlyListResolvesWithAllRegistered"/>.
        /// </summary>
        private static IEnumerable<TestCaseData> ReadOnlyListTestCaseSource()
        {
            {
                var services = new ServiceCollection();

                services.AddSingleton<ClassWithIReadOnlyList>();
                services.AddNamed<string, ITest>(ServiceLifetime.Singleton)
                    .Add<T1>("T1") // +
                    .Add<T2>("TTT") // +
                    .Add<T2>("T2") // +
                    .Add(typeof(T1), "T1-1") // +
                    .Add(typeof(T2), sp => sp.GetRequiredService<T2>(), "T2-1-Factory") // +
                    .Add<T2>(sp => sp.GetRequiredService<T2>(), "T2-1-Generic-Factory") // +
                    .Add<T1>(sp => sp.GetRequiredService<T1>()); // +

                yield return new TestCaseData("FirstTestCase", 7, services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<ClassWithIReadOnlyList>();
                services.AddNamed<string, ITest>(ServiceLifetime.Singleton)
                    .Add<T1>() // +
                    .Add<T1>("T1") // +
                    .Add<T2>("T2") // +
                    .Add(typeof(T2), "T2-1") // +
                    .Add(typeof(T2), sp => sp.GetRequiredService<T2>(), "T2-1-Factory") // +
                    .Add<T2>(sp => sp.GetRequiredService<T2>(), "T2-1-Generic-Factory") // +
                    .Add(typeof(T2), "TTT"); // +

                yield return new TestCaseData("SecondTestCase", 7, services.BuildServiceProvider());
            }
        }

        /// <summary>
        /// Тест кейсы для <seealso cref="ClassWithIEnumerableResolvesWithAllRegistered"/>.
        /// </summary>
        private static IEnumerable<TestCaseData> EnumerableTestCaseSource()
        {
            {
                var services = new ServiceCollection();

                services.AddSingleton<ClassWithEnumerable>();
                services.AddNamed<string, ITest>(ServiceLifetime.Singleton)
                    .Add<T1>("T1")
                    .Add<T2>("TTT")
                    .Add<T2>("T2")
                    .Add(typeof(T1), "T1-1")
                    .Add(typeof(T2), sp => sp.GetRequiredService<T2>(), "T2-1-Factory")
                    .Add<T2>(sp => sp.GetRequiredService<T2>(), "T2-1-Generic-Factory")
                    .Add<T1>(sp => sp.GetRequiredService<T1>()); // +

                // there are only one default - only one will be resolved by IEnumerable<ITest>
                yield return new TestCaseData("FirstTestCase", 1, services.BuildServiceProvider());
            }

            {
                var services = new ServiceCollection();

                services.AddSingleton<ClassWithEnumerable>();
                services.AddNamed<string, ITest>(ServiceLifetime.Singleton)
                    .Add<T1>() // +
                    .Add<T1>("T1")
                    .Add<T2>("T2")
                    .Add(typeof(T2), "T2-1")
                    .Add(typeof(T2), sp => sp.GetRequiredService<T2>(), "T2-1-Factory")
                    .Add<T2>(sp => sp.GetRequiredService<T2>(), "T2-1-Generic-Factory")
                    .Add(typeof(T2), "TTT");

                // T1 registered as default, and with name T1. in IEnumerable<ITest> only one 1.
                yield return new TestCaseData("SecondTestCase", 1, services.BuildServiceProvider());
            }
        }

        #endregion Тест кейсы
    }
}