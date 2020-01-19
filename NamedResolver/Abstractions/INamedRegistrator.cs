using System;

namespace NamedResolver.Abstractions
{
    /// <summary>
    /// Интерфейс регистратора именованных типов.
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    public interface INamedRegistrator<TInterface>
    {
        /// <summary>
        /// Зарегистрировать тип.
        /// </summary>
        /// <param name="name">Имя инстанса.</param>
        /// <param name="type">Тип.</param>
        /// <returns>Регистратор именованных типов.</returns>
        INamedRegistrator<TInterface> Add(string name, Type type);
    }
}
