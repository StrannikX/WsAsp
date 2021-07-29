using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WsAsp
{
    /// <summary>
    /// Middleware for websocket handlers.
    /// </summary>
    /// <typeparam name="THandler">Type of handler</typeparam>
    internal sealed class WsMiddleware<THandler> where THandler : WsHandler
    {
        /// <summary>
        /// Next request delegate.
        /// </summary>
        private readonly RequestDelegate _next;

        /// <summary>
        /// Creates middleware.
        /// </summary>
        /// <param name="next">Next request delegate.</param>
        public WsMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        /// <summary>
        /// Invokes middleware.
        /// </summary>
        /// <param name="context">Http context.</param>
        /// <param name="dispatcher">Websocket connection dispatcher for <typeparamref name="THandler"/></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, WsConnectionDispatcher<THandler> dispatcher)
        {
            await dispatcher.DispatchConnectionAsync(context).ConfigureAwait(false);
        }
    }
}