using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WsAsp
{
    internal sealed class WsMiddleware<THandler> where THandler : WsHandler
    {
        private readonly RequestDelegate _next;

        public WsMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context, WsConnectionDispatcher<THandler> dispatcher)
        {
            await dispatcher.DispatchConnectionAsync(context);
        }
    }
}