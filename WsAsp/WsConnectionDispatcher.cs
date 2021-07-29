using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WsAsp
{
    /// <summary>
    /// Websocket connection dispatcher.
    /// </summary>
    /// <typeparam name="THandler">Type of web socket handler.</typeparam>
    internal sealed class WsConnectionDispatcher<THandler> where THandler : WsHandler
    {

        /// <summary>
        /// Websockets manager.
        /// </summary>
        private readonly WsManager<THandler> _manager;

        /// <summary>
        /// Websockets handler.
        /// </summary>
        private readonly THandler _handler;

        /// <summary>
        /// Websockets options object.
        /// </summary>
        private readonly WsOptions _options;

        /// <summary>
        /// Creates handler.
        /// </summary>
        /// <param name="manager">Websockets manager.</param>
        /// <param name="handler">Websockets handler.</param>
        /// <param name="options">Websockets options object.</param>
        public WsConnectionDispatcher(WsManager<THandler> manager, THandler handler, WsOptions options)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Asynchronously dispatches websocket connection.
        /// </summary>
        /// <param name="context"><see cref="HttpContext"/> of initial request.</param>
        public async Task DispatchConnectionAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = 400;
                return;
            }

            using var socket = await context.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);

            _manager.AddSocket(socket);

            _handler.Socket = socket;
            _handler.Manager = _manager;

            await _handler.OnConnectedAsync(context).ConfigureAwait(false);
            await StartMessageLoopAsync(socket, async (result, buffer, count) =>
            {
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    var closeMessage = new WsCloseMessage(result.CloseStatus, result.CloseStatusDescription);
                    await _handler.OnDisconnectedAsync(closeMessage).ConfigureAwait(false);
                    return;
                }

                try
                {
                    var message = new WsReceiveMessage(result.MessageType, buffer, count);
                    await _handler.OnReceiveAsync(message).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    await socket
                        .CloseAsync(WebSocketCloseStatus.InternalServerError, e.Message, CancellationToken.None)
                        .ConfigureAwait(false);
                }
            }).ConfigureAwait(false);

            _manager.RemoveSocket(socket);
        }

        /// <summary>
        /// Starts message reading loop for <paramref name="socket"/>.
        /// </summary>
        /// <param name="socket">Websocket to read from.</param>
        /// <param name="handleMessage">Message handling delegate.</param>
        private async Task StartMessageLoopAsync(WebSocket socket, Func<WebSocketReceiveResult, byte[], int, Task> handleMessage)
        {
            var buffer = new byte[_options.BufferSize];

            while (socket.State == WebSocketState.Open)
            {
                var offset = 0;
                var free = buffer.Length;
                WebSocketReceiveResult result;

                while (true)
                {
                    result = await socket
                        .ReceiveAsync(new ArraySegment<byte>(buffer, offset, free), CancellationToken.None)
                        .ConfigureAwait(false);

                    offset += result.Count;
                    free -= result.Count;
                    
                    if (result.EndOfMessage || free != 0) break;

                    var newSize = buffer.Length + _options.BufferSize;

                    if (newSize > _options.MaxBufferSize)
                    {
                        throw new ArgumentOutOfRangeException(nameof(result), "Websocket message has length greater then MaxBufferSize");
                    }

                    var newBuffer = new byte[newSize];
                    Array.Copy(buffer, 0, newBuffer, 0, offset);
                    buffer = newBuffer;
                    free = buffer.Length - offset;
                }

                await handleMessage(result, buffer, offset).ConfigureAwait(false);
            }
        }
    }
}