using System;
using System.Collections.Generic;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Интерфейс для получения информации о зарегистрированных типов.
    /// </summary>
    internal interface IHasRegisteredTypeInfos<TDiscriminator, TInterface>
    {
        /// <summary>
        /// Словарь зарегистрированных типов.
        /// </summary>
        IReadOnlyDictionary<TDiscriminator, Type> RegisteredTypes { get; }

        /// <summary>
        /// Словарь зарегистрированных фабрик типов.
        /// </summary>
        IReadOnlyDictionary<TDiscriminator, Func<IServiceProvider, TInterface>> RegisteredTypesFactories { get; }

        /// <summary>
        /// Тип зарегистрированный по-умолчанию.
        /// </summary>
        Type DefaultType { get; }

        /// <summary>
        /// Фабрика типа по-умолчанию.
        /// </summary>
        Func<IServiceProvider, TInterface> DefaultTypeFactory { get; }

        /// <summary>
        /// Механизм сравнения дискриминаторов.
        /// </summary>
        IEqualityComparer<TDiscriminator> EqualityComparer { get; }
    }
}
