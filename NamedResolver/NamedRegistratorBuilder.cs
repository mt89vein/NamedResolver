using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NamedResolver.Abstractions;
using System;

namespace NamedResolver
{
    /// <summary>
    /// Конфгуратор именованных типов.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    internal sealed class NamedRegistratorBuilder<TInterface> : INamedRegistratorBuilder<TInterface>
        where TInterface : class
    {
        #region Поля

        /// <summary>
        /// Конфигуратор сервисов.
        /// </summary>
        private readonly IServiceCollection _services;

        /// <summary>
        /// Жизненный цикл типа.
        /// </summary>
        private readonly ServiceLifetime _serviceLifetime;

        /// <summary>
        /// Регистратор именованных типов.
        /// </summary>
        private readonly INamedRegistrator<TInterface> _namedRegistrator;

        #endregion Поля

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса <see cref="NamedRegistratorBuilder" />.
        /// </summary>
        /// <param name="services">Конфигуратор сервисов.</param>
        /// <param name="namedRegistrator">Регистратор именованных типов.</param>
        /// <param name="serviceLifetime">Жизненный цикл типа.</param>
        internal NamedRegistratorBuilder(
            IServiceCollection services,
            INamedRegistrator<TInterface> namedRegistrator,
            ServiceLifetime serviceLifetime
        )
        {
            _namedRegistrator = namedRegistrator;
            _services = services;
            _serviceLifetime = serviceLifetime;
        }

        #endregion Конструктор

        #region Методы (public)

        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="name">Имя типа.</param>
        /// <returns>Конфгуратор именованных типов.</returns>
        public INamedRegistratorBuilder<TInterface> Add(Type type, string name = null)
        {
            _namedRegistrator.Add(name, type);

            _services.TryAdd(new ServiceDescriptor(type, type, _serviceLifetime));
            if (name == null)
            {
                _services.TryAdd(new ServiceDescriptor(
                    typeof(TInterface),
                    sp => sp.GetRequiredService(type),
                    _serviceLifetime)
                );
            }

            return this;
        }

        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="factory">Фабрика получения типа.</param>
        /// <param name="name">Имя типа.</param>
        /// <returns>Конфгуратор именованных типов.</returns>
        public INamedRegistratorBuilder<TInterface> Add(Type type, Func<IServiceProvider, object> factory, string name = null)
        {
            _namedRegistrator.Add(name, type);

            _services.TryAdd(new ServiceDescriptor(type, type, _serviceLifetime));
            if (name == null)
            {
                _services.TryAdd(new ServiceDescriptor(
                    typeof(TInterface),
                    factory,
                    _serviceLifetime)
                );
            }

            return this;
        }

        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <typeparam name="TImplementation">Тип реализации.</typeparam>
        /// <param name="name">Имя типа.</param>
        /// <returns>Конфгуратор именованных типов.</returns>
        public INamedRegistratorBuilder<TInterface> Add<TImplementation>(string name = null)
            where TImplementation : class, TInterface
        {
            return Add(typeof(TImplementation), name);
        }

        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <typeparam name="TImplementation">Тип реализации.</typeparam>
        /// <param name="factory">Фабрика получения типа.</param>
        /// <param name="name">Имя типа.</param>
        /// <returns>Конфгуратор именованных типов.</returns>
        public INamedRegistratorBuilder<TInterface> Add<TImplementation>(Func<IServiceProvider, object> factory, string name = null)
            where TImplementation : class, TInterface
        {
            return Add(typeof(TImplementation), factory, name);
        }

        #endregion Методы (public)
    }
}
