using System.Collections.Generic;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Интерфейс для получения информации о зарегистрированных типов.
    /// </summary>
    internal interface IHasRegisteredTypeInfos<TDiscriminator, TInterface>
        where TInterface : class
    {
        /// <summary>
        /// Словарь зарегистрированных дескрипторов типов.
        /// </summary>
        IReadOnlyDictionary<TDiscriminator, NamedDescriptor<TDiscriminator, TInterface>> RegisteredTypes { get; }

        /// <summary>
        /// Дефолтный тип.
        /// </summary>
        NamedDescriptor<TDiscriminator, TInterface>? DefaultDescriptor { get; }

        /// <summary>
        /// Механизм сравнения дискриминаторов.
        /// </summary>
        IEqualityComparer<TDiscriminator?> EqualityComparer { get; }
    }
}
