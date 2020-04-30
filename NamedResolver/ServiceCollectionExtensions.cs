using Microsoft.Extensions.DependencyInjection;
using NamedResolver.Abstractions;
using System.Collections.Generic;

namespace NamedResolver
{
    /// <summary>
    /// Методы расширения для <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Зарегистрировать именованные типы.
        /// </summary>
        /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
        /// <typeparam name="TDiscriminator">
        /// Тип, по которому можно однозначно определить конкретную реализацию <see cref="TInterface"/>.
        /// </typeparam>
        /// <param name="services">Конфигуратор сервисов.</param>
        /// <param name="equalityComparer">Механизм сравнения дискриминаторов.</param>
        /// <param name="serviceLifetime">Жизненный цикл сервисов.</param>
        /// <returns>Конфигуратор именованных типов.</returns>
        public static INamedRegistratorBuilder<TDiscriminator, TInterface> AddNamed<TDiscriminator, TInterface>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped,
            IEqualityComparer<TDiscriminator> equalityComparer = null
        )   where TInterface : class
        {
            var comparer = equalityComparer ?? EqualityComparer<TDiscriminator>.Default;
            var registrator = new NamedRegistrator<TDiscriminator, TInterface>(comparer);
            var builder = new NamedRegistratorBuilder<TDiscriminator, TInterface>(services, registrator, comparer, serviceLifetime);

            services.AddSingleton<INamedRegistrator<TDiscriminator, TInterface>>(registrator);

            services.Add(new ServiceDescriptor(
                    typeof(INamedResolver<TDiscriminator, TInterface>),
                    typeof(NamedResolver<TDiscriminator, TInterface>),
                    serviceLifetime
                )
            );

            services.Add(new ServiceDescriptor(
                typeof(ResolveNamed<TDiscriminator, TInterface>),
                sp => new ResolveNamed<TDiscriminator, TInterface>(sp
                    .GetRequiredService<INamedResolver<TDiscriminator, TInterface>>().Get),
                serviceLifetime
            ));

            services.Add(new ServiceDescriptor(
                    typeof(IReadOnlyList<TInterface>),
                    sp => sp.GetRequiredService<INamedResolver<TDiscriminator, TInterface>>().GetAll(),
                    serviceLifetime
                )
            );

            return builder;
        }
    }
}
