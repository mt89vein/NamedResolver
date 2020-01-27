using System;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Конфгуратор именованных типов.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    public interface INamedRegistratorBuilder<TInterface>
        where TInterface : class
    {
        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="name">Имя типа.</param>
        /// <returns>Конфгуратор именованных типов.</returns>
        INamedRegistratorBuilder<TInterface> Add(Type type, string name = null);

        /// <summary>
        /// Попытаться зарегистрировать тип с указанным именем.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Конфгуратор именованных типов.</returns>
        INamedRegistratorBuilder<TInterface> TryAdd(Type type, string name = null);

        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="factory">Фабрика получения типа.</param>
        /// <param name="name">Имя типа.</param>
        /// <returns>Конфгуратор именованных типов.</returns>
        INamedRegistratorBuilder<TInterface> Add(Type type, Func<IServiceProvider, object> factory, string name = null);

        /// <summary>
        /// Попытаться зарегистрировать тип с указанным именем.
        /// </summary>
        /// <param name="type">Тип.</param>
        /// <param name="factory">Фабрика получения типа.</param>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Конфгуратор именованных типов.</returns>
        INamedRegistratorBuilder<TInterface> TryAdd(Type type, Func<IServiceProvider, object> factory, string name = null);

        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <typeparam name="TImplementation">Тип реализации.</typeparam>
        /// <param name="name">Имя типа.</param>
        /// <returns>Конфгуратор именованных типов.</returns>
        INamedRegistratorBuilder<TInterface> Add<TImplementation>(string name = null)
            where TImplementation : class, TInterface;

        /// <summary>
        /// Попытаться зарегистрировать тип с указанным именем.
        /// </summary>
        /// <typeparam name="TImplementation">Тип реализации.</typeparam>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Конфгуратор именованных типов.</returns>
        INamedRegistratorBuilder<TInterface> TryAdd<TImplementation>(string name = null)
            where TImplementation : class, TInterface;

        /// <summary>
        /// Зарегистрировать тип с указанным именем.
        /// </summary>
        /// <typeparam name="TImplementation">Тип реализации.</typeparam>
        /// <param name="factory">Фабрика получения типа.</param>
        /// <param name="name">Имя типа.</param>
        /// <returns>Конфгуратор именованных типов.</returns>
        INamedRegistratorBuilder<TInterface> Add<TImplementation>(Func<IServiceProvider, object> factory, string name = null)
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
        /// <returns>Конфгуратор именованных типов.</returns>
        INamedRegistratorBuilder<TInterface> TryAdd<TImplementation>(Func<IServiceProvider, object> factory, string name = null)
            where TImplementation : class, TInterface;
    }
}
