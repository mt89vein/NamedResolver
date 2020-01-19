using System.Collections.Generic;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Интерфейс резолвера именнованого типа.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    public interface INamedResolver<TInterface>
    {
        /// <summary>
        /// Индексатор для получения реализации по имени.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <returns>Инстанс, или <see cref="default{TInterface}" /> если реализация не зарегистрирована.</returns>
        TInterface this[string name] { get; }

        /// <summary>
        /// Получить реализацию по-имени.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Инстанс.</returns>
        TInterface GetRequired(string name = null);

        /// <summary>
        /// Получить реализацию по имени.
        /// </summary>
        /// <param name="name">Имя типа.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб
        /// из-за некорректного состояния провайдера служб.
        /// т.к. вероятно была перерегистрация или очистка после настройки.
        /// </exception>
        /// <returns>
        /// Инстанс, или <see cref="default{TInterface}" /> если реализация не зарегистрирована.
        /// </returns>
        TInterface Get(string name = null);

        /// <summary>
        /// Попытаться получить реализацию по имени.
        /// </summary>
        /// <param name="instance">Инстанс.</param>
        /// <param name="name">Имя инстанса.</param>
        /// <returns>True, если удалось получить инстанс, False в противном случае.</returns>
        bool TryGet(out TInterface instance, string name = null);

        /// <summary>
        /// Получить все зарегистрированные типы.
        /// </summary>
        /// <returns>Список (имя типа, инстанс)</returns>
        IReadOnlyList<(string name, TInterface instance)> GetAllWithNames();

        /// <summary>
        /// Получить все зарегистрированные типы.
        /// </summary>
        /// <returns>Список инстансов.</returns>
        IReadOnlyList<TInterface> GetAll();
    }
}
