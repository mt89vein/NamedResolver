using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NamedResolver.Abstractions;
using System;
using System.Collections.Generic;

namespace NamedResolver
{
    /// <summary>
    /// Конфигуратор именованных типов.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    /// <typeparam name="TDiscriminator">
    /// Тип, по которому можно однозначно определить конкретную реализацию <see cref="TInterface"/>.
    /// </typeparam>
    internal sealed class NamedRegistratorBuilder<TDiscriminator, TInterface>
        : INamedRegistratorBuilder<TDiscriminator, TInterface>
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
        private readonly INamedRegistrator<TDiscriminator, TInterface> _namedRegistrator;

        /// <summary>
        /// Механизм сравнения дискриминаторов.
        /// </summary>
        private readonly IEqualityComparer<TDiscriminator> _equalityComparer;

        #endregion Поля

        #region Конструктор

        /// <summary>
        /// Создает экземпляр класса <see cref="NamedRegistratorBuilder{TDiscriminator, TInterface}" />.
        /// </summary>
        /// <param name="services">Конфигуратор сервисов.</param>
        /// <param name="namedRegistrator">Регистратор именованных типов.</param>
        /// <param name="equalityComparer">Механизм сравнения дискриминаторов.</param>
        /// <param name="serviceLifetime">Жизненный цикл типа.</param>
        internal NamedRegistratorBuilder(
            IServiceCollection services,
            INamedRegistrator<TDiscriminator, TInterface> namedRegistrator,
            IEqualityComparer<TDiscriminator> equalityComparer,
            ServiceLifetime serviceLifetime
        )
        {
            _namedRegistrator = namedRegistrator;
            _equalityComparer = equalityComparer;
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
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Если тип с таким именем уже зарегистрирован.
        /// </exception>
        /// <returns>Конфигуратор именованных типов.</returns>
        public INamedRegistratorBuilder<TDiscriminator, TInterface> Add(Type type, TDiscriminator name = default)
        {
            _namedRegistrator.Add(name, type);

            _services.TryAdd(new ServiceDescriptor(type, type, _serviceLifetime));
            if (_equalityComparer.Equals(name, default))
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
        /// Попытаться зарегистрировать тип с указанным именем.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Конфигуратор именованных типов.</returns>
        public INamedRegistratorBuilder<TDiscriminator, TInterface> TryAdd(Type type, TDiscriminator name = default)
        {
            if (_namedRegistrator.TryAdd(name, type))
            {
                _services.TryAdd(new ServiceDescriptor(type, type, _serviceLifetime));
                if (_equalityComparer.Equals(name, default))
                {
                    _services.TryAdd(new ServiceDescriptor(
                        typeof(TInterface),
                        sp => sp.GetRequiredService(type),
                        _serviceLifetime)
                    );
                }
            }

            return this;
        }

        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="factory">Фабрика получения типа.</param>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Если тип с таким именем уже зарегистрирован.
        /// </exception>
        /// <returns>Конфигуратор именованных типов.</returns>
        /// <remarks>
        /// Фабрика будет вызываться на каждый резолв! Из-за особенностей стандартного DI.
        /// </remarks>
        public INamedRegistratorBuilder<TDiscriminator, TInterface> Add(Type type, Func<IServiceProvider, TInterface> factory, TDiscriminator name = default)
        {
            _namedRegistrator.Add(name, factory);

            _services.TryAdd(new ServiceDescriptor(type, factory, _serviceLifetime));
            if (_equalityComparer.Equals(name, default))
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
        /// Попытаться зарегистрировать тип с указанным именем.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="factory">Фабрика получения типа.</param>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Конфигуратор именованных типов.</returns>
        /// <remarks>
        /// Фабрика будет вызываться на каждый резолв! Из-за особенностей стандартного DI.
        /// </remarks>
        public INamedRegistratorBuilder<TDiscriminator, TInterface> TryAdd(Type type, Func<IServiceProvider, TInterface> factory, TDiscriminator name = default)
        {
            if (_namedRegistrator.TryAdd(name, factory))
            {
                _services.TryAdd(new ServiceDescriptor(type, factory, _serviceLifetime));
                if (_equalityComparer.Equals(name, default))
                {
                    _services.TryAdd(new ServiceDescriptor(
                        typeof(TInterface),
                        factory,
                        _serviceLifetime)
                    );
                }
            }

            return this;
        }

        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <typeparam name="TImplementation">Тип реализации.</typeparam>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Если тип с таким именем уже зарегистрирован.
        /// </exception>
        /// <returns>Конфигуратор именованных типов.</returns>
        public INamedRegistratorBuilder<TDiscriminator, TInterface> Add<TImplementation>(TDiscriminator name = default)
            where TImplementation : class, TInterface
        {
            return Add(typeof(TImplementation), name);
        }

        /// <summary>
        /// Попытаться зарегистрировать тип с указанным именем.
        /// </summary>
        /// <typeparam name="TImplementation">Тип реализации.</typeparam>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Конфигуратор именованных типов.</returns>
        public INamedRegistratorBuilder<TDiscriminator, TInterface> TryAdd<TImplementation>(TDiscriminator name = default)
            where TImplementation : class, TInterface
        {
            return TryAdd(typeof(TImplementation), name);
        }

        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <typeparam name="TImplementation">Тип реализации.</typeparam>
        /// <param name="factory">Фабрика получения типа.</param>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Если тип с таким именем уже зарегистрирован.
        /// </exception>
        /// <returns>Конфигуратор именованных типов.</returns>
        /// <remarks>
        /// Фабрика будет вызываться на каждый резолв! Из-за особенностей стандартного DI.
        /// </remarks>
        public INamedRegistratorBuilder<TDiscriminator, TInterface> Add<TImplementation>(Func<IServiceProvider, TInterface> factory, TDiscriminator name = default)
            where TImplementation : class, TInterface
        {
            return Add(typeof(TImplementation), factory, name);
        }

        /// <summary>
        /// Попытаться зарегистрировать тип с указанным именем.
        /// </summary>
        /// <typeparam name="TImplementation">Тип реализации.</typeparam>
        /// <param name="factory">Фабрика получения типа.</param>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Конфигуратор именованных типов.</returns>
        /// <remarks>
        /// Фабрика будет вызываться на каждый резолв! Из-за особенностей стандартного DI.
        /// </remarks>
        public INamedRegistratorBuilder<TDiscriminator, TInterface> TryAdd<TImplementation>(Func<IServiceProvider, TInterface> factory, TDiscriminator name = default)
            where TImplementation : class, TInterface
        {
            return TryAdd(typeof(TImplementation), factory, name);
        }

        #endregion Методы (public)
    }
}
