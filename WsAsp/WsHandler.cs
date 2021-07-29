using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WsAsp
{
    /// <summary>
    /// Websocket handler
    /// </summary>
    public abstract class WsHandler
    {
        /// <summary>
        /// Websocket
        /// </summary>
        protected internal WebSocket? Socket { get; set; } = null;

        /// <summary>
        /// Websocket connections manager
        /// </summary>
        internal WsManager? Manager { get; set; } = null;

        /// <summary>
        /// Handle websocket connection.
        /// </summary>
        /// <param name="context">Http context of websocket initial http request.</param>
        public abstract Task OnConnectedAsync(HttpContext context);

        /// <summary>
        /// Handle websocket disconnected event.
        /// </summary>
        /// <param name="message">Message with websocket closing data.</param>
        public abstract Task OnDisconnectedAsync(WsCloseMessage message);

        /// <summary>
        /// Handle received message.
        /// </summary>
        /// <param name="message">Received message.</param>
        public abstract Task OnReceiveAsync(WsReceiveMessage message);

        #region SendTextAsync

        /// <summary>
        /// Sends text <paramref name="message"/> encoded with <see cref="Encoding.UTF8"/> to current websocket connection.
        /// </summary>
        /// <param name="message">Text of message.</param>
        /// <param name="token">Cancellation token</param>
        protected async Task SendTextAsync(string message, CancellationToken token = default)
        {
            if (Socket is null)
            {
                throw new ArgumentNullException(nameof(Socket));
            }
            await Socket.SendTextAsync(message, Encoding.UTF8, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends text <paramref name="message"/> to current websocket connection.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="encoding">Encoding of message. <see cref="Encoding.UTF8"/> if null.</param>
        /// <param name="token">Cancellation token</param>
        protected async Task SendTextAsync(string message, Encoding? encoding, CancellationToken token = default)
        {
            if (Socket is null)
            {
                throw new ArgumentNullException(nameof(Socket));
            }
            await Socket.SendTextAsync(message, encoding, token).ConfigureAwait(false);
        }
        #endregion

        #region SendBinaryDataAsync

        /// <summary>
        /// Sends binary <paramref name="data"/> to current websocket connection.
        /// </summary>
        /// <param name="data">Data bytes array.</param>
        /// <param name="token">Cancellation token</param>
        protected async Task SendBinaryDataAsync(byte[] data, CancellationToken token = default)
        {
            await SendBinaryDataAsync(data, 0, data.Length, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends binary <paramref name="data"/> to current websocket connection.
        /// </summary>
        /// <param name="data">Data bytes array.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="count">Count of bytes to send.</param>
        protected async Task SendBinaryDataAsync(byte[] data, int count, CancellationToken token = default)
        {
            await SendBinaryDataAsync(data, 0, count, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends binary <paramref name="data"/> to current websocket connection.
        /// </summary>
        /// <param name="data">Data bytes array.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="offset">Offset in data array.</param>
        /// <param name="count">Count of bytes to send.</param>
        protected async Task SendBinaryDataAsync(byte[] data, int offset, int count, CancellationToken token = default)
        {
            if (Socket is null)
            {
                throw new ArgumentNullException(nameof(Socket));
            }

            await Socket.SendBinaryDataAsync(data, offset, count, token).ConfigureAwait(false);
        }
        #endregion

        #region SendBinaryDataToAllAsyncAsync

        /// <summary>
        /// Sends binary <paramref name="data"/> to all websockets connections working with handler of current type.
        /// </summary>
        /// <param name="data">Data bytes array.</param>
        /// <param name="token">Cancellation token.</param>
        protected async Task SendBinaryDataToAllAsyncAsync(byte[] data, CancellationToken token = default)
        {
            await SendBinaryDataToAllAsyncAsync(data, 0, data.Length, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends binary <paramref name="data"/> to all websockets connections working with handler of current type.
        /// </summary>
        /// <param name="data">Data bytes array.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="count">Count of bytes to send.</param>
        protected async Task SendBinaryDataToAllAsyncAsync(byte[] data, int count, CancellationToken token = default)
        {
            await SendBinaryDataToAllAsyncAsync(data, 0, count, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends binary <paramref name="data"/> to all websockets connections working with handler of current type.
        /// </summary>
        /// <param name="data">Data bytes array.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="offset">Offset in data array.</param>
        /// <param name="count">Count of bytes to send.</param>
        protected async Task SendBinaryDataToAllAsyncAsync(byte[] data, int offset, int count, CancellationToken token = default)
        {
            if (Manager is null)
            {
                throw new ArgumentNullException(nameof(Manager));
            }
            await Manager.SendBinaryDataToAllAsync(data, offset, count, token).ConfigureAwait(false);
        }
        #endregion

        #region SendTextToAllAsync

        /// <summary>
        /// Sends text <paramref name="message"/> to all websockets connections working with handler of current type.
        /// </summary>
        /// <param name="message">Text of message.</param>
        /// <param name="encoding">Encoding of message. <see cref="Encoding.UTF8"/> if null.</param>
        /// <param name="token">Cancellation token.</param>
        public async Task SendTextToAllAsync(string message, Encoding? encoding = null, CancellationToken token = default)
        {
            if (Manager is null)
            {
                throw new ArgumentNullException(nameof(Manager));
            }

            await Manager.SendTextToAllAsync(message, encoding, token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends text <paramref name="message"/> encoded with <see cref="Encoding.UTF8"/> to all websockets connections working with handler of current type.
        /// </summary>
        /// <param name="message">Text of message.</param>
        /// <param name="token">Cancellation token.</param>
        public async Task SendTextToAllAsync(string message, CancellationToken token = default)
        {
            await SendTextToAllAsync(message, Encoding.UTF8, token).ConfigureAwait(false);
        }
        #endregion

        #region CloseAsync

        /// <summary>
        /// Closes websocket.
        /// </summary>
        /// <param name="closeMessage">Message.</param>
        /// <param name="token">Cancellation token.</param>
        protected async Task CloseAsync(string? closeMessage = null, CancellationToken token = default)
        {
            if (Socket == null)
            {
                throw new InvalidOperationException("Operation on null socket!");
            }

            if (Socket.State != WebSocketState.Open)
                return;

            await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, closeMessage ?? String.Empty, token).ConfigureAwait(false);
        }
        #endregion
    }
}