using System;
using System.Collections.Generic;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Интерфейс для получения информации о зарегистрированных типов.
    /// </summary>
    internal interface IHasRegisteredTypeInfos
    {
        /// <summary>
        /// Словарь зарегистрированных типов.
        /// </summary>
        IReadOnlyDictionary<string, Type> RegisteredTypes { get; }

        /// <summary>
        /// Тип зарегистрированный по-умолчанию.
        /// </summary>
        Type DefaultType { get; }
    }
}
