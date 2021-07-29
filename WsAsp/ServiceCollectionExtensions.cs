using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace WsAsp
{
    /// <summary>
    /// Provides extension methods for registering WsAsp handlers in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds WsAsp services to the container.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the WsAsp to.</param>
        /// <param name="optionsBuilder">Delegate to configure <see cref="WsOptions"/></param>
        /// <returns>An instance of <see cref="IHealthChecksBuilder"/> from which WsAsp services can be registered.</returns>
        /// <exception cref="ArgumentNullException"/>
        public static IServiceCollection AddWsAsp(
            this IServiceCollection serviceCollection,
            Action<WsOptions>? optionsBuilder = null)
        {
            if (serviceCollection is null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            var options = new WsOptions();
            optionsBuilder?.Invoke(options);
            serviceCollection.AddSingleton(options);
            return serviceCollection;
        }

        /// <summary>
        /// Adds WsAsp handler of type <typeparamref name="THandler"/> to the container.
        /// </summary>
        /// <typeparam name="THandler">Type of WsAsp handler.</typeparam>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the handler to.</param>
        /// <returns>An instance of <see cref="IHealthChecksBuilder"/> from which WsAsp handler can be registered.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static IServiceCollection AddWsAspHandler<THandler>(this IServiceCollection serviceCollection) where THandler : WsHandler
        {
            if (serviceCollection is null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            var handlerType = typeof(THandler);
            return serviceCollection.AddWsAspHandler(handlerType);
        }

        /// <summary>
        /// Adds WsAsp handler of type <paramref name="handlerType"/> to the container.
        /// </summary>
        /// <param name="handlerType">Type of WsAsp handler.</param>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the handler to.</param>
        /// <returns>An instance of <see cref="IHealthChecksBuilder"/> from which WsAsp handler can be registered.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static IServiceCollection AddWsAspHandler(this IServiceCollection serviceCollection, Type handlerType)
        {
            if (serviceCollection is null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (handlerType is null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }

            if (!typeof(WsHandler).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException($"{handlerType.FullName} is not an inheritor of {typeof(WsHandler).FullName}");
            }

            serviceCollection.AddTransient(handlerType);

            var managerType = typeof(WsManager<>).MakeGenericType(handlerType);
            serviceCollection.AddSingleton(managerType);

            var dispatcherType = typeof(WsConnectionDispatcher<>).MakeGenericType(handlerType);
            serviceCollection.AddTransient(dispatcherType);

            return serviceCollection;
        }

        /// <summary>
        /// Adds all WsAsp handlers from assemblies determined by <paramref name="assemblyMarkers"/>.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> to add the handlers to.</param>
        /// <param name="assemblyMarkers">Types from assemblies from which to register handlers.</param>
        /// <returns>An instance of <see cref="IHealthChecksBuilder"/> from which WsAsp handlers can be registered.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static IServiceCollection AddWsAspHandlers(
            this IServiceCollection serviceCollection,
            params Type[] assemblyMarkers)
        {
            if (serviceCollection is null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (assemblyMarkers is null)
            {
                throw new ArgumentNullException(nameof(assemblyMarkers));
            }

            var types = assemblyMarkers
                .Select(marker => marker.Assembly)
                .Distinct()
                .SelectMany(assembly => assembly.ExportedTypes)
                .Where(t => typeof(WsHandler).IsAssignableFrom(t));

            foreach (var type in types)
            {
                serviceCollection.AddWsAspHandler(type);
            }

            return serviceCollection;
        }
    }
}