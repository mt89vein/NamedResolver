using System;

namespace NamedResolver
{
    /// <summary>
    /// Делегат-резолвер именованного типа.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    /// <typeparam name="TDiscriminator">
    /// Тип, по которому можно однозначно определить конкретную реализацию <see cref="TInterface"/>.
    /// </typeparam>
    /// <summary>
    /// Получить реализацию по дискриминатору.
    /// </summary>
    /// <param name="name">Дискриминатор типа.</param>
    /// <exception cref="InvalidOperationException">
    /// Если не удалось получить инстанс из провайдера служб
    /// из-за некорректного состояния провайдера служб.
    /// т.к. вероятно была перерегистрация или очистка после настройки.
    /// </exception>
    /// <returns>Инстанс, или default если реализация не зарегистрирована.</returns>
    public delegate TInterface ResolveNamed<in TDiscriminator, out TInterface>(TDiscriminator name = default);
}