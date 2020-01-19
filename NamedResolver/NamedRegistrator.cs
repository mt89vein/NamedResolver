using NamedResolver.Abstractions;
using System;
using System.Collections.Generic;

namespace NamedResolver
{
    /// <summary>
    /// Регистратор именованных типов.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    internal sealed class NamedRegistrator<TInterface> : INamedRegistrator<TInterface>, IHasRegisteredTypeInfos
        where TInterface : class
    {
        #region Поля, свойства

        /// <summary>
        /// Словарь зарегистрированных типов.
        /// </summary>
        private readonly Dictionary<string, Type> _instanceTypes = new Dictionary<string, Type>();

        /// <summary>
        /// Словарь зарегистрированных типов.
        /// </summary>
        public IReadOnlyDictionary<string, Type> RegisteredTypes => _instanceTypes;

        /// <summary>
        /// Тип по-умолчанию.
        /// </summary>
        public Type DefaultType { get; private set; }

        #endregion Поля, свойства

        #region Методы (public)

        /// <summary>
        /// Зарегистрировать тип.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <param name="type">Тип.</param>
        /// <returns>Регистратор именованных типов.</returns>
        public INamedRegistrator<TInterface> Add(string name, Type type)
        {
            if (!typeof(TInterface).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"Тип {type.FullName} не реализует интерфейс {typeof(TInterface).FullName}");
            }

            if (string.IsNullOrEmpty(name))
            {
                if (DefaultType != null)
                {
                    throw new InvalidOperationException($"Тип с именем {name} уже зарегистрирован");
                }

                DefaultType = type;

                return this;
            }

            if (!_instanceTypes.ContainsKey(name))
            {
                _instanceTypes.Add(name, type);

                return this;
            }

            throw new InvalidOperationException($"Тип с именем {name} уже зарегистрирован");
        }

        #endregion Методы (public)
    }
}
