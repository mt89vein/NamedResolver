using System;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Конфигуратор именованных типов.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    /// <typeparam name="TDiscriminator">
    /// Тип, по которому можно однозначно определить конкретную реализацию <see cref="TInterface"/>.
    /// </typeparam>
    public interface INamedRegistratorBuilder<in TDiscriminator, in TInterface>
        where TInterface : class
    {
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
        INamedRegistratorBuilder<TDiscriminator, TInterface> Add(Type type, TDiscriminator? name = default);

        /// <summary>
        /// Попытаться зарегистрировать тип с указанным именем.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Конфигуратор именованных типов.</returns>
        INamedRegistratorBuilder<TDiscriminator, TInterface> TryAdd(Type type, TDiscriminator? name = default);

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
        INamedRegistratorBuilder<TDiscriminator, TInterface> Add(Type type, Func<IServiceProvider, TInterface> factory, TDiscriminator? name = default);

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
        INamedRegistratorBuilder<TDiscriminator, TInterface> TryAdd(Type type, Func<IServiceProvider, TInterface> factory, TDiscriminator? name = default);

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
        INamedRegistratorBuilder<TDiscriminator, TInterface> Add<TImplementation>(TDiscriminator? name = default)
            where TImplementation : class, TInterface;

        /// <summary>
        /// Попытаться зарегистрировать тип с указанным именем.
        /// </summary>
        /// <typeparam name="TImplementation">Тип реализации.</typeparam>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Конфигуратор именованных типов.</returns>
        INamedRegistratorBuilder<TDiscriminator, TInterface> TryAdd<TImplementation>(TDiscriminator? name = default)
            where TImplementation : class, TInterface;

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
        INamedRegistratorBuilder<TDiscriminator, TInterface> Add<TImplementation>(Func<IServiceProvider, TInterface> factory, TDiscriminator? name = default)
            where TImplementation : class, TInterface;

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
        INamedRegistratorBuilder<TDiscriminator, TInterface> TryAdd<TImplementation>(Func<IServiceProvider, TInterface> factory, TDiscriminator? name = default)
            where TImplementation : class, TInterface;
    }
}
