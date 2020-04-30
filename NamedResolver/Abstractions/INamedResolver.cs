using System;
using System.Collections.Generic;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Интерфейс резолвера именованного типа.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    /// <typeparam name="TDiscriminator">
    /// Тип, по которому можно однозначно определить конкретную реализацию <see cref="TInterface"/>.
    /// </typeparam>
    public interface INamedResolver<TDiscriminator, TInterface>
    {
        /// <summary>
        /// Индексатор для получения реализации по дискриминатору.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <returns>Инстанс, или default если реализация не зарегистрирована.</returns>
        TInterface this[TDiscriminator name] { get; }

        /// <summary>
        /// Получить реализацию по-дискриминатору.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Инстанс.</returns>
        TInterface GetRequired(TDiscriminator name = default);

        /// <summary>
        /// Получить реализацию по дискриминатору.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб
        /// из-за некорректного состояния провайдера служб.
        /// т.к. вероятно была перерегистрация или очистка после настройки.
        /// </exception>
        /// <returns>
        /// Инстанс, или default если реализация не зарегистрирована.
        /// </returns>
        TInterface Get(TDiscriminator name = default);

        /// <summary>
        /// Попытаться получить реализацию по дискриминатору.
        /// </summary>
        /// <param name="instance">Инстанс.</param>
        /// <param name="name">Имя инстанса.</param>
        /// <returns>True, если удалось получить инстанс, False в противном случае.</returns>
        bool TryGet(out TInterface instance, TDiscriminator name = default);

        /// <summary>
        /// Получить все зарегистрированные типы.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Список (имя типа, инстанс)</returns>
        IReadOnlyList<(TDiscriminator name, TInterface instance)> GetAllWithNames(Func<TDiscriminator, Type, bool> predicate = null);

        /// <summary>
        /// Получить все зарегистрированные типы.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Список инстансов.</returns>
        IReadOnlyList<TInterface> GetAll(Func<Type, bool> predicate = null);
    }
}
