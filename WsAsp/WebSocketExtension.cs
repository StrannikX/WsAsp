using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WsAsp
{
    /// <summary>
    /// <see cref="WebSocket"/> extensions methods.
    /// </summary>
    internal static class WebSocketExtension
    {
        /// <summary>
        /// Sends binary <paramref name="data"/> to <paramref name="socket"/>.
        /// </summary>
        /// <param name="socket">Websocket to send data.</param>
        /// <param name="data">Data bytes array.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="offset">Offset in data array.</param>
        /// <param name="count">Count of bytes to send.</param>
        public static async Task SendBinaryDataAsync(this WebSocket? socket, byte[] data, int offset, int count, CancellationToken token)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (socket.State != WebSocketState.Open)
                return;

            await socket.SendAsync(
                buffer: new ArraySegment<byte>(data, offset, count),
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: token).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends text <paramref name="message"/> to <paramref name="socket"/>
        /// </summary>
        /// <param name="socket">Websocket to send data.</param>
        /// <param name="message">Message</param>
        /// <param name="encoding">Encoding of message. <see cref="Encoding.UTF8"/> if null.</param>
        /// <param name="token">Cancellation token</param>
        public static async Task SendTextAsync(this WebSocket socket, string message, Encoding? encoding = null, CancellationToken token = default)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (socket.State != WebSocketState.Open)
                return;

            encoding ??= Encoding.UTF8;

            var buffer = encoding.GetBytes(message);
            await socket.SendAsync(
                buffer: new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: token).ConfigureAwait(false);
        }
    }
}