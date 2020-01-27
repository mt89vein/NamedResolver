using Microsoft.Extensions.DependencyInjection;
using NamedResolver.Abstractions;

namespace NamedResolver
{
    /// <summary>
    /// Методы расширения для <see cref="IServiceCollection" />.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Зарегистрировать именнованые типы.
        /// </summary>
        /// <typeparam name="TInterface">Тип интерфейса.</typeparam>
        /// <param name="services">Конфигуратор сервисов.</param>
        /// <param name="serviceLifetime">Жизненный цикл сервисов.</param>
        /// <returns>Конфгиратор именнованных типов.</returns>
        public static INamedRegistratorBuilder<TInterface> AddNamed<TInterface>(
            this IServiceCollection services,
            ServiceLifetime serviceLifetime = ServiceLifetime.Scoped
        ) where TInterface : class
        {
            var registrator = new NamedRegistrator<TInterface>();
            var builder = new NamedRegistratorBuilder<TInterface>(services, registrator, serviceLifetime);

            services.AddSingleton<INamedRegistrator<TInterface>>(registrator);

            services.Add(new ServiceDescriptor(
                typeof(INamedResolver<TInterface>),
                typeof(NamedResolver<TInterface>),
                serviceLifetime)
            );

            return builder;
        }
    }
}
