using System;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Интерфейс регистратора именованных типов.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    public interface INamedRegistrator<TInterface>
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
        void Add(string name, Type type);

        /// <summary>
        /// Попытаться зарегистрировать тип, если еще не зарегистрировано.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <param name="type">Тип.</param>
        /// <exception cref="InvalidOperationException">
        /// Если параметр type не реализует интерфейс <see cref="TInterface" />.
        /// </exception>
        /// <returns>Регистратор именованных типов.</returns>
        bool TryAdd(string name, Type type);
    }
}
