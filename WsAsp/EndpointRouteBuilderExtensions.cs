using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace WsAsp
{
    /// <summary>
    /// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add websocket handlers.
    /// </summary>
    public static class EndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Adds a websocket handler endpoint to <see cref="IEndpointRouteBuilder"/> with specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the health checks endpoint to.</param>
        /// <param name="pattern">The URL pattern of the websocket handler endpoint.</param>
        /// <typeparam name="THandler">Type of websocket handler.</typeparam>
        /// <returns>A convention routes for the websocket handler endpoint.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public static IEndpointConventionBuilder MapWsAspHandler<THandler>(this IEndpointRouteBuilder endpoints,
            string pattern)
            where THandler : WsHandler => endpoints.MapWsAspHandler(pattern, typeof(THandler));

        /// <summary>
        /// Adds a websocket handler endpoint to <see cref="IEndpointRouteBuilder"/> with specified pattern.
        /// </summary>
        /// <param name="endpoints">The <see cref="IEndpointRouteBuilder"/> to add the health checks endpoint to.</param>
        /// <param name="pattern">The URL pattern of the websocket handler endpoint.</param>
        /// <param name="handlerType">Type of websocket handler.</param>
        /// <returns>A convention routes for the websocket handler endpoint.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidOperationException"/>
        public static IEndpointConventionBuilder MapWsAspHandler(this IEndpointRouteBuilder endpoints, string pattern, Type handlerType)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }

            if (handlerType is null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }

            if (!typeof(WsHandler).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException($"{handlerType.FullName} is not an inheritor of {typeof(WsHandler).FullName}");
            }

            var options = endpoints.ServiceProvider.GetService(typeof(WsOptions));
            if (options is null)
            {
                throw new InvalidOperationException($"WsAsp is not configured! Add \"services.{nameof(ServiceCollectionExtensions.AddWsAsp)}(...)\" to Startup.ConfigureServices(...)");
            }

            var dispatcherType = typeof(WsConnectionDispatcher<>).MakeGenericType(handlerType);
            var handlerDispatcher = endpoints.ServiceProvider.GetService(dispatcherType);
            if (handlerDispatcher is null)
            {
                throw new InvalidOperationException($"Handler is not configured! Add \"services.{nameof(ServiceCollectionExtensions.AddWsAspHandler)}<{handlerType.Name}>(...)\" to Startup.ConfigureServices(...)");
            }

            var middlewareType = typeof(WsMiddleware<>).MakeGenericType(handlerType);
            var pipeline = endpoints.CreateApplicationBuilder()
                .UseMiddleware(middlewareType)
                .Build();

            return endpoints.MapGet(pattern, pipeline);
        }
    }
}