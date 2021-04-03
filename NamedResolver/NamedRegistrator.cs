using NamedResolver.Abstractions;
using System;
using System.Collections.Generic;

namespace NamedResolver
{
    /// <summary>
    /// Регистратор именованных типов.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    /// <typeparam name="TDiscriminator">
    /// Тип, по которому можно однозначно определить конкретную реализацию <see cref="TInterface"/>.
    /// </typeparam>
    internal sealed class NamedRegistrator<TDiscriminator, TInterface>
        : INamedRegistrator<TDiscriminator, TInterface>,
            IHasRegisteredTypeInfos<TDiscriminator, TInterface>
        where TInterface : class
    {
        #region Поля, свойства

        /// <summary>
        /// Мутабельный словарь зарегистрированных дескрипторов типов.
        /// </summary>
        private readonly Dictionary<TDiscriminator, NamedDescriptor<TDiscriminator, TInterface>> _types;

        /// <summary>
        /// Словарь зарегистрированных дескрипторов типов.
        /// </summary>
        public IReadOnlyDictionary<TDiscriminator, NamedDescriptor<TDiscriminator, TInterface>> RegisteredTypes => _types;

        /// <summary>
        /// Дескриптор по-умолчанию.
        /// </summary>
        public NamedDescriptor<TDiscriminator, TInterface>? DefaultDescriptor { get; private set; }

        /// <summary>
        /// Механизм сравнения дискриминаторов.
        /// </summary>
        public IEqualityComparer<TDiscriminator?> EqualityComparer { get; }

        #endregion Поля, свойства

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса <see cref="NamedRegistrator{TDiscriminator,TInterface}"/>.
        /// </summary>
        /// <param name="equalityComparer">Механизм сравнения дискриминаторов.</param>
        public NamedRegistrator(IEqualityComparer<TDiscriminator?> equalityComparer)
        {
            EqualityComparer = equalityComparer;
            _types = new Dictionary<TDiscriminator, NamedDescriptor<TDiscriminator, TInterface>>(EqualityComparer);
            DefaultDescriptor = null;
        }

        #endregion Конструктор

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
        public void Add(TDiscriminator? name, Type type)
        {
            if (!typeof(TInterface).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"Тип {type.FullName} не реализует интерфейс {typeof(TInterface).FullName}");
            }

            if (EqualityComparer.Equals(name, default))
            {
                if (DefaultDescriptor.HasValue)
                {
                    throw new InvalidOperationException("Тип с именем по-умолчанию уже зарегистрирован");
                }

                DefaultDescriptor = new NamedDescriptor<TDiscriminator, TInterface>(default, EqualityComparer, type);

                return;
            }

            if (_types.ContainsKey(name!))
            {
                throw new InvalidOperationException($"Тип с именем {name} уже зарегистрирован");
            }

            _types.Add(name!, new NamedDescriptor<TDiscriminator, TInterface>(name, EqualityComparer, type));
        }

        /// <summary>
        /// Зарегистрировать тип.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <param name="factory">Фабрика типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если тип с таким именем уже зарегистрирован.
        /// </exception>
        /// <returns>Регистратор именованных типов.</returns>
        public void Add(TDiscriminator? name, Func<IServiceProvider, TInterface> factory)
        {
            if (EqualityComparer.Equals(name, default))
            {
                if (DefaultDescriptor.HasValue)
                {
                    throw new InvalidOperationException("Тип с именем по-умолчанию уже зарегистрирован");
                }

                DefaultDescriptor = new NamedDescriptor<TDiscriminator, TInterface>(default, EqualityComparer, typeFactory: factory);

                return;
            }

            if (_types.ContainsKey(name!))
            {
                throw new InvalidOperationException($"Тип с именем {name} уже зарегистрирован");
            }

            _types.Add(name!, new NamedDescriptor<TDiscriminator, TInterface>(name, EqualityComparer, typeFactory: factory));
        }

        /// <summary>
        /// Попытаться зарегистрировать тип, если еще не зарегистрировано.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <param name="factory">Фабрика типа.</param>
        /// <returns>Регистратор именованных типов.</returns>
        public bool TryAdd(TDiscriminator? name, Func<IServiceProvider, TInterface> factory)
        {
            if (EqualityComparer.Equals(name, default))
            {
                if (DefaultDescriptor.HasValue)
                {
                    return false;
                }

                DefaultDescriptor = new NamedDescriptor<TDiscriminator, TInterface>(default, EqualityComparer, typeFactory: factory);

                return true;
            }

            if (_types.ContainsKey(name!))
            {
                return false;
            }

            _types.Add(name!, new NamedDescriptor<TDiscriminator, TInterface>(name, EqualityComparer, typeFactory: factory));

            return true;
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
        public bool TryAdd(TDiscriminator? name, Type type)
        {
            if (!typeof(TInterface).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"Тип {type.FullName} не реализует интерфейс {typeof(TInterface).FullName}");
            }

            if (EqualityComparer.Equals(name, default))
            {
                if (DefaultDescriptor.HasValue)
                {
                    return false;
                }

                DefaultDescriptor = new NamedDescriptor<TDiscriminator, TInterface>(default, EqualityComparer, type);

                return true;
            }

            if (_types.ContainsKey(name!))
            {
                return false;
            }

            _types.Add(name!, new NamedDescriptor<TDiscriminator, TInterface>(name, EqualityComparer, type));

            return true;
        }

        #endregion Методы (public)
    }
}
