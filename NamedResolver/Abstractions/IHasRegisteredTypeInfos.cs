using System;
using System.Collections.Generic;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Интерфейс для получения информации о зарегистрированных типов.
    /// </summary>
    internal interface IHasRegisteredTypeInfos<TInterface>
    {
        /// <summary>
        /// Словарь зарегистрированных типов.
        /// </summary>
        IReadOnlyDictionary<string, Type> RegisteredTypes { get; }

        /// <summary>
        /// Словарь зарегистрированных фабрик типов.
        /// </summary>
        IReadOnlyDictionary<string, Func<IServiceProvider, TInterface>> RegisteredTypesFactories { get; }

        /// <summary>
        /// Тип зарегистрированный по-умолчанию.
        /// </summary>
        Type DefaultType { get; }

        /// <summary>
        /// Фабрика типа по-умолчанию.
        /// </summary>
        Func<IServiceProvider, TInterface> DefaultTypeFactory { get; }
    }
}
