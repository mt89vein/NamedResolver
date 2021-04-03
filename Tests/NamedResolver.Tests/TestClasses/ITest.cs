using System;
using System.Collections.Generic;

namespace NamedResolver.Tests.TestClasses
{
    public interface ITest
    {
    }

    public class T1 : ITest
    {
        public Guid Id { get; set; } = Guid.NewGuid();

    }

    public class T2 : ITest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class T3 // не реализует ITest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public class DependentClass
    {
        public ITest Test { get; }

        public DependentClass(ITest test)
        {
            Test = test;
        }
    }

    public enum EnumForResolver
    {
        Default = 0,
        T1 = 1,
        T2 = 2,
        T1_1 = 3,
        T2_1 = 4,
        T2_1_Factory = 5,
        T2_1_Generic_Factory = 6,
        T2_2_Generic_Factory = 7,
        TTT = 8,
    }

    /// <summary>
    /// For correct using class or struct, you should implement
    /// <see cref="IEquatable{CustomClassForResolver}"/> or provide
    /// <seealso cref="IEqualityComparer{CustomClassForResolver}"/> into registration.
    /// </summary>
    public class CustomClassForResolver
    {
        public string Value { get; }

        private CustomClassForResolver(string value)
        {
            Value = value;
        }

        public static CustomClassForResolver CreateWith(string value) =>
            value == null
                ? null
                : new CustomClassForResolver(value);

        #region EqualityComparer

        private sealed class CustomEqualityComparer : IEqualityComparer<CustomClassForResolver>
        {
            public bool Equals(CustomClassForResolver x, CustomClassForResolver y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.Value == y.Value;
            }

            public int GetHashCode(CustomClassForResolver obj)
            {
                return (obj.Value != null ? obj.Value.GetHashCode() : 0);
            }
        }

        public static IEqualityComparer<CustomClassForResolver> CustomComparer { get; } = new CustomEqualityComparer();

        #endregion EqualityComparer
    }
}