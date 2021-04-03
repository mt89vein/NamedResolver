using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace NamedResolver
{
    /// <summary>
    /// Дескриптор типа.
    /// </summary>
    /// <typeparam name="TDiscriminator">Дескриминатор.</typeparam>
    /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
    internal readonly struct NamedDescriptor<TDiscriminator, TInterface>
        : IEquatable<NamedDescriptor<TDiscriminator, TInterface>>
        where TInterface : class
    {
        #region Поля

        /// <summary>
        /// Механизм сравнения дискриминатора.
        /// </summary>
        private readonly IEqualityComparer<TDiscriminator?>? _equalityComparer;

        #endregion Поля

        #region Свойства

        /// <summary>
        /// Дискриминатор.
        /// </summary>
        public TDiscriminator? Discriminator { get; }

        /// <summary>
        /// Тип.
        /// </summary>
        public Type? Type { get; }

        /// <summary>
        /// Фабрика типа.
        /// </summary>
        public Func<IServiceProvider, TInterface>? TypeFactory { get; }

        #endregion Свойства

        #region Конструктор

        /// <summary>
        /// Создает новый экземпляр структуры <seealso cref="NamedDescriptor{TDiscriminator,TInterface}"/>.
        /// </summary>
        public NamedDescriptor(
            TDiscriminator? discriminator,
            IEqualityComparer<TDiscriminator?>? equalityComparer = default,
            Type? type = default,
            Func<IServiceProvider, TInterface>? typeFactory = default
        )
        {
            _equalityComparer = equalityComparer ?? EqualityComparer<TDiscriminator?>.Default;
            Discriminator = discriminator;
            Type = type;
            TypeFactory = typeFactory;
        }

        #endregion Конструктор

        #region Методы (public)

        /// <summary>
        /// Получить инстанс из провайдера служб.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <returns>Инстанс.</returns>
        public TInterface Resolve(IServiceProvider serviceProvider)
        {
            var instance = Type != null
                ? (TInterface) serviceProvider.GetRequiredService(Type)
                : TypeFactory!(serviceProvider);

            return instance ??
                   throw new InvalidOperationException(
                       $"Не удалось получить тип с именем {Discriminator}, ServiceProvider вернул null."
                   );
        }

        /// <summary>
        /// Получить инстанс из провайдера служб.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        /// <param name="instance">Инстанс.</param>
        public bool TryResolve(IServiceProvider serviceProvider, out TInterface? instance)
        {
            instance = Type != null
                ? (TInterface) serviceProvider.GetService(Type)
                : TypeFactory!(serviceProvider);

            return instance != null;
        }

        /// <summary>
        /// Резолвит из <see cref="IServiceProvider"/>, если удовлетворяет указанному предикату
        /// в противном случае, возвращает default.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        /// <param name="predicate">Предикат.</param>
        /// <param name="instance">Инстанс.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <remarks>
        /// Если тип зарегистрирован как фабрика, то нужно выполнить резолв, чтобы
        /// выполнить проверку на предикат.
        /// </remarks>
        public bool TryResolveIfSatisfiedBy(
            IServiceProvider serviceProvider,
            Func<Type, bool> predicate,
            out TInterface? instance
        )
        {
            if (Type != null)
            {
                if (predicate(Type))
                {
                    instance = Resolve(serviceProvider);

                    return true;
                }

                instance = default;

                return false;
            }

            instance = Resolve(serviceProvider);

            if (predicate(instance.GetType()))
            {
                return true;
            }

            instance = default;

            return false;
        }

        /// <summary>
        /// Резолвит из <see cref="IServiceProvider"/>, если удовлетворяет указанному предикату
        /// в противном случае, возвращает default.
        /// </summary>
        /// <param name="serviceProvider">Провайдер служб.</param>
        /// <param name="predicate">Предикат.</param>
        /// <param name="instance">Инстанс.</param>
        /// <exception cref="InvalidOperationException">
        /// Если не удалось получить инстанс из провайдера служб.
        /// </exception>
        /// <remarks>
        /// Если тип зарегистрирован как фабрика, то нужно выполнить резолв, чтобы
        /// выполнить проверку на предикат.
        /// </remarks>
        public bool TryResolveIfSatisfiedBy(
            IServiceProvider serviceProvider,
            Func<TDiscriminator?, Type, bool> predicate,
            out TInterface? instance
        )
        {
            if (Type != null)
            {
                if (predicate(Discriminator, Type))
                {
                    instance = Resolve(serviceProvider);

                    return true;
                }

                instance = default;

                return false;
            }

            instance = Resolve(serviceProvider);

            if (predicate(Discriminator, instance.GetType()))
            {
                return true;
            }

            instance = default;

            return false;
        }

        #endregion Методы (public)

        #region IEquatable support

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(NamedDescriptor<TDiscriminator, TInterface> other)
        {
            return (_equalityComparer ?? EqualityComparer<TDiscriminator?>.Default).Equals(Discriminator, other.Discriminator);
        }

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            return obj is NamedDescriptor<TDiscriminator, TInterface> other && Equals(other);
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return (_equalityComparer ?? EqualityComparer<TDiscriminator?>.Default).GetHashCode(Discriminator);
        }

        /// <summary>Returns a value that indicates whether the values of two <see cref="T:NamedResolver.NamedDescriptor`2" /> objects are equal.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if the <paramref name="left" /> and <paramref name="right" /> parameters have the same value; otherwise, false.</returns>
        public static bool operator ==(NamedDescriptor<TDiscriminator, TInterface> left, NamedDescriptor<TDiscriminator, TInterface> right)
        {
            return left.Equals(right);
        }

        /// <summary>Returns a value that indicates whether two <see cref="T:NamedResolver.NamedDescriptor`2" /> objects have different values.</summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns>true if <paramref name="left" /> and <paramref name="right" /> are not equal; otherwise, false.</returns>
        public static bool operator !=(NamedDescriptor<TDiscriminator, TInterface> left, NamedDescriptor<TDiscriminator, TInterface> right)
        {
            return !left.Equals(right);
        }

        #endregion IEquatable support
    }
}