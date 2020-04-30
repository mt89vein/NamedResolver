using Microsoft.Extensions.DependencyInjection;
using NamedResolver.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NamedResolver
{
    /// <summary>
    /// Резолвер именованного типа.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    /// <typeparam name="TDiscriminator">
    /// Тип, по которому можно однозначно определить конкретную реализацию <see cref="TInterface"/>.
    /// </typeparam>
    public sealed class NamedResolver<TDiscriminator, TInterface>
        : INamedResolver<TDiscriminator, TInterface>
        where TInterface : class
    {
        #region Поля

        /// <summary>
        /// Провайдер сервисов.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Список зарегистрированных типов.
        /// </summary>
        private readonly IReadOnlyDictionary<TDiscriminator, Type> _instanceTypes;

        /// <summary>
        /// Список зарегистрированных фабрик типов.
        /// </summary>
        private readonly IReadOnlyDictionary<TDiscriminator, Func<IServiceProvider, TInterface>> _instanceTypeFactories;

        /// <summary>
        /// Тип по-умолчанию.
        /// </summary>
        private readonly Type _defaultType;

        /// <summary>
        /// Фабрика типа по-умолчанию.
        /// </summary>
        private readonly Func<IServiceProvider, TInterface> _defaultTypeFactory;

        /// <summary>
        /// Механизм сравнения дискриминаторов.
        /// </summary>
        private readonly IEqualityComparer<TDiscriminator> _equalityComparer;

        #endregion Поля

        #region Индексаторы

        /// <summary>
        /// Индексатор для получения реализации по дискриминатору.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Инстанс, или default если реализация не зарегистрирована.</returns>
        public TInterface this[TDiscriminator name] => Get(name);

        #endregion Индексаторы

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса <see cref="INamedResolver{TDiscriminator, TInterface}"/>.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        /// <param name="namedRegistrator">Регистратор именованных типов.</param>
        public NamedResolver(IServiceProvider serviceProvider, INamedRegistrator<TDiscriminator, TInterface> namedRegistrator)
        {
            _serviceProvider = serviceProvider;
            var registeredTypesAccessor = (IHasRegisteredTypeInfos<TDiscriminator, TInterface>) namedRegistrator;
            _instanceTypes = registeredTypesAccessor.RegisteredTypes;
            _defaultType = registeredTypesAccessor.DefaultType;
            _instanceTypeFactories = registeredTypesAccessor.RegisteredTypesFactories;
            _defaultTypeFactory = registeredTypesAccessor.DefaultTypeFactory;
            _equalityComparer = registeredTypesAccessor.EqualityComparer;
        }

        #endregion Конструктор

        #region Методы (public)

        /// <summary>
        /// Получить реализацию по-дискриминатору.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Инстанс.</returns>
        public TInterface GetRequired(TDiscriminator name = default)
        {
            if (!TryGet(out var instance, name))
            {
                if (_equalityComparer.Equals(name, default))
                {
                    throw new InvalidOperationException($"Не удалось получить реализацию по-умолчанию для {typeof(TInterface).FullName}.");
                }
                else
                {
                    throw new InvalidOperationException($"Не удалось получить инстанс с именем {name} для {typeof(TInterface).FullName}.");
                }
            }

            return instance;
        }

        /// <summary>
        /// Получить реализацию по дискриминатору.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб
        /// из-за некорректного состояния провайдера служб.
        /// т.к. вероятно была перерегистрация или очистка после настройки.
        /// </exception>
        /// <returns>Инстанс, или default если реализация не зарегистрирована.</returns>
        public TInterface Get(TDiscriminator name = default)
        {
            if (_equalityComparer.Equals(name, default))
            {
                if (_defaultType != null)
                {
                    return Resolve(_defaultType);
                }

                if (_defaultTypeFactory != null)
                {
                    return Resolve(_defaultTypeFactory);
                }

                return default;
            }

            if (_instanceTypes.TryGetValue(name, out var type))
            {
                return Resolve(type);
            }

            if (_instanceTypeFactories.TryGetValue(name, out var factory))
            {
                return Resolve(factory);
            }

            return default;
        }

        /// <summary>
        /// Попытаться получить реализацию по дискриминатору.
        /// </summary>
        /// <param name="instance">Инстанс.</param>
        /// <param name="name">Имя инстанса.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>true, если удалось получить инстанс, false в противном случае.</returns>
        public bool TryGet(out TInterface instance, TDiscriminator name = default)
        {
            instance = default;

            if (_equalityComparer.Equals(name, default))
            {
                if (_defaultType != null)
                {
                    instance = ResolveSafe(_defaultType);
                }

                if (_defaultTypeFactory != null)
                {
                    instance = ResolveSafe(_defaultTypeFactory);
                }

                return instance != null;
            }

            if (_instanceTypes.TryGetValue(name, out var type))
            {
                instance = ResolveSafe(type);

                return instance != null;
            }

            if (_instanceTypeFactories.TryGetValue(name, out var factory))
            {
                instance = ResolveSafe(factory);

                return instance != null;
            }

            return false;
        }

        /// <summary>
        /// Получить все зарегистрированные типы.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Список инстансов.</returns>
        public IReadOnlyList<TInterface> GetAll(Func<Type, bool> predicate = null)
        {
            var res = new List<TInterface>();
            foreach (var type in _instanceTypes.Select(t => t.Value))
            {
                if (predicate == null || predicate(type))
                {
                    res.Add(Resolve(type));
                }
            }

            foreach (var factory in _instanceTypeFactories.Select(t => t.Value))
            {
                var resolvedTypeInstance = Resolve(factory);
                if (predicate == null || predicate(resolvedTypeInstance.GetType()))
                {
                    res.Add(resolvedTypeInstance);
                }
            }

            if (_defaultType != null)
            {
                if (predicate == null || predicate(_defaultType))
                {
                    res.Add(Resolve(_defaultType));
                }
            }

            if (_defaultTypeFactory != null)
            {
                var resolvedTypeInstance = Resolve(_defaultTypeFactory);
                if (predicate == null || predicate(resolvedTypeInstance.GetType()))
                {
                    res.Add(resolvedTypeInstance);
                }
            }

            return res;
        }

        /// <summary>
        /// Получить все зарегистрированные типы.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Список (имя типа, инстанс)</returns> 
        public IReadOnlyList<(TDiscriminator name, TInterface instance)> GetAllWithNames(Func<TDiscriminator, Type, bool> predicate = null)
        {
            var res = new List<(TDiscriminator, TInterface)>();

            foreach (var type in _instanceTypes)
            {
                if (predicate == null || predicate(type.Key, type.Value))
                {
                    res.Add((type.Key, Resolve(type.Value)));
                }
            }

            foreach (var typeFactory in _instanceTypeFactories)
            {
                var resolvedTypeInstance = Resolve(typeFactory.Value);
                if (predicate == null || predicate(typeFactory.Key, resolvedTypeInstance.GetType()))
                {
                    res.Add((typeFactory.Key, resolvedTypeInstance));
                }
            }

            if (_defaultType != null)
            {
                if (predicate == null || predicate(default, _defaultType))
                {
                    res.Add((default, Resolve(_defaultType)));
                }
            }

            if (_defaultTypeFactory != null)
            {
                var resolvedTypeInstance = Resolve(_defaultTypeFactory);
                if (predicate == null || predicate(default, resolvedTypeInstance.GetType()))
                {
                    res.Add((default, resolvedTypeInstance));
                }
            }

            return res;
        }

        #endregion Методы (public)

        #region Методы (private)

        /// <summary>
        /// Получить инстанс из провайдера служб.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Инстанс.</returns>
        private TInterface Resolve(Type type)
        {
            return _serviceProvider.GetRequiredService(type) as TInterface;
        }

        /// <summary>
        /// Получить инстанс из провайдера служб.
        /// </summary>
        /// <param name="factory">Фабрика типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Инстанс.</returns>
        private TInterface Resolve(Func<IServiceProvider, TInterface> factory)
        {
            return factory(_serviceProvider);
        }

        /// <summary>
        /// Получить инстанс из провайдера служб.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <returns>Инстанс.</returns>
        private TInterface ResolveSafe(Type type)
        {
            return _serviceProvider.GetService(type) as TInterface;
        }

        /// <summary>
        /// Получить инстанс из провайдера служб.
        /// </summary>
        /// <param name="factory">Фабрика типа.</param>
        /// <returns>Инстанс.</returns>
        private TInterface ResolveSafe(Func<IServiceProvider, TInterface> factory)
        {
            return factory(_serviceProvider);
        }

        #endregion Методы (private)
    }
}
