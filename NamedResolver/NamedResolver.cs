using Microsoft.Extensions.DependencyInjection;
using NamedResolver.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NamedResolver
{
    /// <summary>
    /// Резолвер именнованого типа.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    public sealed class NamedResolver<TInterface> : INamedResolver<TInterface>
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
        private readonly IReadOnlyDictionary<string, Type> _instanceTypes;

        /// <summary>
        /// Тип по-умолчанию.
        /// </summary>
        private readonly Type _defaultType;

        #endregion Поля

        #region Индексаторы

        /// <summary>
        /// Индексатор для получения реализации по имени.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Инстанс, или <see cref="default{TInterface}" /> если реализация не зарегистрирована.</returns>
        public TInterface this[string name] => Get(name);

        #endregion Индексаторы

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса <see cref="INamedResolver{TInterface}"/>.
        /// </summary>
        /// <param name="serviceProvider">Провадер служб.</param>
        /// <param name="namedRegistrator">Регистратор именованных типов.</param>
        public NamedResolver(IServiceProvider serviceProvider, INamedRegistrator<TInterface> namedRegistrator)
        {
            _serviceProvider = serviceProvider;
            _instanceTypes = (namedRegistrator as IHasRegisteredTypeInfos).RegisteredTypes;
            _defaultType = (namedRegistrator as IHasRegisteredTypeInfos).DefaultType;
        }

        #endregion Конструктор

        #region Методы (public)

        /// <summary>
        /// Получить реализацию по-имени.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Инстанс.</returns>
        public TInterface GetRequired(string name = null)
        {
            if (!TryGet(out var instance, name))
            {
                if (string.IsNullOrEmpty(name))
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
        /// Получить реализацию по имени.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб
        /// из-за некорректного состояния провайдера служб.
        /// т.к. вероятно была перерегистрация или очистка после настройки.
        /// </exception>
        /// <returns>Инстанс, или <see cref="default{TInterface}" /> если реализация не зарегистрирована.</returns>
        public TInterface Get(string name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                if (_defaultType == null)
                {
                    return default;
                }

                return Resolve(_defaultType);
            }

            if (_instanceTypes.TryGetValue(name, out var type))
            {
                return Resolve(type);
            }

            return default;
        }

        /// <summary>
        /// Попытаться получить реализацию по имени.
        /// </summary>
        /// <param name="instance">Инстанс.</param>
        /// <param name="name">Имя инстанса.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>true, если удалось получить инстанс, false в противном случае.</returns>
        public bool TryGet(out TInterface instance, string name = null)
        {
            instance = default;

            if (string.IsNullOrEmpty(name))
            {
                instance = _defaultType != null
                    ? ResolveSafe(_defaultType)
                    : default;

                return instance != null;
            }

            if (_instanceTypes.TryGetValue(name, out var type))
            {
                instance = ResolveSafe(type);

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
        public IReadOnlyList<TInterface> GetAll()
        {
            var res = new List<TInterface>();
            foreach (var type in _instanceTypes.Select(t => t.Value))
            {
                res.Add(Resolve(type));
            }

            if (_defaultType != null && !_instanceTypes.Values.Any(t => t == _defaultType))
            {
                res.Add(Resolve(_defaultType));
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
        public IReadOnlyList<(string name, TInterface instance)> GetAllWithNames()
        {
            var res = new List<(string, TInterface)>();

            foreach (var type in _instanceTypes)
            {
                res.Add((type.Key, Resolve(type.Value)));
            }

            if (_defaultType != null && !_instanceTypes.Values.Any(t => t == _defaultType))
            {
                res.Add((string.Empty, Resolve(_defaultType)));
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
        /// <param name="type">Тип.</param>
        /// <returns>Инстанс.</returns>
        private TInterface ResolveSafe(Type type)
        {
            return _serviceProvider.GetService(type) as TInterface;
        }

        #endregion Методы (private)
    }
}
