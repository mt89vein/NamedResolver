using System;

namespace NamedRegistration
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

    public class DependentClass
    {
        public ITest Test { get; }

        public DependentClass(ITest test)
        {
            Test = test;
        }
    }
}
