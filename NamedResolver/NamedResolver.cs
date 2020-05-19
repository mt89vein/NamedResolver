using NamedResolver.Abstractions;
using System;
using System.Collections.Generic;

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
        private readonly IReadOnlyDictionary<TDiscriminator, NamedDescriptor<TDiscriminator, TInterface>> _registeredDescriptors;

        /// <summary>
        /// Дефолтный дескриптор.
        /// </summary>
        private readonly NamedDescriptor<TDiscriminator, TInterface>? _defaultDescriptor;

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
            _registeredDescriptors = registeredTypesAccessor.RegisteredTypes;
            _defaultDescriptor = registeredTypesAccessor.DefaultDescriptor;
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
                return _defaultDescriptor?.Resolve(_serviceProvider);
            }

            return _registeredDescriptors.TryGetValue(name, out var namedDescriptor)
                ? namedDescriptor.Resolve(_serviceProvider)
                : default;
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
            if (_equalityComparer.Equals(name, default))
            {
                if (_defaultDescriptor != null)
                {
                    return _defaultDescriptor.Value.TryResolve(_serviceProvider, out instance);
                }

                instance = default;

                return false;
            }

            if (_registeredDescriptors.TryGetValue(name, out var namedDescriptor))
            {
                return namedDescriptor.TryResolve(_serviceProvider, out instance);
            }

            instance = default;

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
            var instances = new List<TInterface>();

            foreach (var descriptor in _registeredDescriptors.Values)
            {
                if (predicate == null)
                {
                    instances.Add(descriptor.Resolve(_serviceProvider));
                }
                else if (descriptor.TryResolveIfSatisfiedBy(_serviceProvider, predicate, out var resolvedInstance))
                {
                    instances.Add(resolvedInstance);
                }
            }

            if (_defaultDescriptor != null)
            {
                if (predicate == null)
                {
                    instances.Add(_defaultDescriptor.Value.Resolve(_serviceProvider));
                }
                else if (_defaultDescriptor.Value.TryResolveIfSatisfiedBy(_serviceProvider, predicate, out var resolvedInstance))
                {
                    instances.Add(resolvedInstance);
                }
            }

            return instances;
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
            var instances = new List<(TDiscriminator, TInterface)>();

            foreach (var keyValuePair in _registeredDescriptors)
            {
                if (predicate == null)
                {
                    instances.Add((keyValuePair.Key, keyValuePair.Value.Resolve(_serviceProvider)));
                }
                else if (keyValuePair.Value.TryResolveIfSatisfiedBy(_serviceProvider, predicate, out var resolvedInstance))
                {
                    instances.Add((keyValuePair.Key, resolvedInstance));
                }
            }

            if (_defaultDescriptor != null)
            {
                if (predicate == null)
                {
                    instances.Add((default, _defaultDescriptor.Value.Resolve(_serviceProvider)));
                }
                else if (_defaultDescriptor.Value.TryResolveIfSatisfiedBy(_serviceProvider, predicate, out var resolvedInstance))
                {
                    instances.Add((default, resolvedInstance));
                }
            }

            return instances;
        }

        #endregion Методы (public)
    }
}
