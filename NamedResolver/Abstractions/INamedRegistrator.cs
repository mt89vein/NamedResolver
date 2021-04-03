using System;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Интерфейс регистратора именованных типов.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    /// <typeparam name="TDiscriminator">
    /// Тип, по которому можно однозначно определить конкретную реализацию <see cref="TInterface"/>.
    /// </typeparam>
    public interface INamedRegistrator<in TDiscriminator, in TInterface>
        where TInterface : class
    {
        /// <summary>
        /// Зарегистрировать тип.
        /// </summary>
        /// <param name="name">Имя инстанса.</param>
        /// <param name="type">Тип.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Если тип с таким именем уже зарегистрирован.
        /// </exception>
        /// <returns>Регистратор именованных типов.</returns>
        void Add(TDiscriminator? name, Type type);

        /// <summary>
        /// Зарегистрировать фабрику.
        /// </summary>
        /// <param name="name">Имя инстанса.</param>
        /// <param name="factory">Фабрика типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если тип с таким именем уже зарегистрирован.
        /// </exception>
        /// <returns>Регистратор именованных типов.</returns>
        void Add(TDiscriminator? name, Func<IServiceProvider, TInterface> factory);

        /// <summary>
        /// Попытаться зарегистрировать тип, если еще не зарегистрировано.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <param name="type">Тип.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Регистратор именованных типов.</returns>
        bool TryAdd(TDiscriminator? name, Type type);

        /// <summary>
        /// Попытаться зарегистрировать тип, если еще не зарегистрировано.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <param name="factory">Фабрика типа.</param>
        /// <returns>Регистратор именованных типов.</returns>
        bool TryAdd(TDiscriminator? name, Func<IServiceProvider, TInterface> factory);
    }
}
