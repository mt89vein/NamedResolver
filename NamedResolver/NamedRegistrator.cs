﻿using NamedResolver.Abstractions;
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
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Если тип с таким именем уже зарегистрирован.
        /// </exception>
        /// <returns>Регистратор именованных типов.</returns>
        public void Add(string name, Type type)
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

                return;
            }

            if (!_instanceTypes.ContainsKey(name))
            {
                _instanceTypes.Add(name, type);

                return;
            }

            throw new InvalidOperationException($"Тип с именем {name} уже зарегистрирован");
        }

        /// <summary>
        /// Попытаться зарегистрировать тип, если еще не зарегистрировано.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <param name="type">Тип.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Регистратор именованных типов.</returns>
        public bool TryAdd(string name, Type type)
        {
            if (!typeof(TInterface).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"Тип {type.FullName} не реализует интерфейс {typeof(TInterface).FullName}");
            }

            if (string.IsNullOrEmpty(name))
            {
                if (DefaultType == null)
                {
                    DefaultType = type;

                    return true;
                }

                return false;
            }

            if (!_instanceTypes.ContainsKey(name))
            {
                _instanceTypes.Add(name, type);

                return true;
            }

            return false;
        }

        #endregion Методы (public)
    }
}
