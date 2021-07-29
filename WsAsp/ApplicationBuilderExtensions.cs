using Microsoft.AspNetCore.Builder;
using System;

namespace WsAsp
{
    /// <summary>
    /// <see cref="IApplicationBuilder"/> extensions methods for WsApp middleware.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a middleware that provides websocket connection.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="path">The path on which to provide websocket connection.</param>
        /// <param name="handlerType">Type of websocket handler.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static IApplicationBuilder MapWsAspHandler(this IApplicationBuilder app, string path, Type handlerType)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (handlerType is null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }

            if (!typeof(WsHandler).IsAssignableFrom(handlerType))
            {
                throw new ArgumentException($"{handlerType.FullName} is not an inheritor of {typeof(WsHandler).FullName}");
            }

            var middlewareType = typeof(WsMiddleware<>).MakeGenericType(handlerType);
            app.UseMiddleware(middlewareType);
            return app;
        }

        /// <summary>
        /// Adds a middleware that provides websocket connection.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/>.</param>
        /// <param name="path">The path on which to provide websocket connection.</param>
        /// <typeparam name="THandler">Type of websocket handler.</typeparam>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public static IApplicationBuilder MapWsAspHandler<THandler>(this IApplicationBuilder app, string path)
            where THandler : WsHandler => app.MapWsAspHandler(path, typeof(THandler));
    }
}