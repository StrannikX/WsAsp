using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WsAsp
{
    /// <summary>
    /// Websocket connections manager.
    /// </summary>
    public abstract class WsManager
    {
        /// <summary>
        /// List of connected WebSockets.
        /// </summary>
        private volatile ImmutableList<WebSocket> _sockets = ImmutableList<WebSocket>.Empty;

        /// <summary>
        /// Member to prevent inheritance of this class in other assemblies.
        /// </summary>
        internal abstract void PreventExternalInheritance();

        /// <summary>
        /// Returns collection of all currently connected websockets.
        /// </summary>
        /// <returns>List of websockets.</returns>
        public IReadOnlyCollection<WebSocket> GetAll()
        {
            return _sockets;
        }

        /// <summary>
        /// Adds websocket.
        /// </summary>
        /// <param name="socket">Websocket to add.</param>
        public void AddSocket(WebSocket socket)
        {
            ImmutableList<WebSocket> newList;
            ImmutableList<WebSocket> oldList;
            do
            {
                oldList = _sockets;
                newList = oldList.Add(socket);
            } while (Interlocked.CompareExchange(ref _sockets, newList, oldList) != oldList);
        }

        /// <summary>
        /// Removes websocket.
        /// </summary>
        /// <param name="socket">Websocket to remove.</param>
        public void RemoveSocket(WebSocket socket)
        {
            ImmutableList<WebSocket> newList;
            ImmutableList<WebSocket> oldList;
            do
            {
                oldList = _sockets;
                newList = oldList.Remove(socket);
            } while (Interlocked.CompareExchange(ref _sockets, newList, oldList) != oldList);
        }

        /// <summary>
        /// Sends binary <paramref name="data"/> to all websockets connections managed by this object.
        /// </summary>
        /// <param name="data">Data bytes array.</param>
        /// <param name="token">Cancellation token.</param>
        /// <param name="offset">Offset in data array.</param>
        /// <param name="count">Count of bytes to send.</param>
        public async Task SendBinaryDataToAllAsync(byte[] data, int offset, int count, CancellationToken token)
        {
            var sockets = _sockets;
            foreach (var socket in sockets)
            {
                await socket.SendBinaryDataAsync(data, offset, count, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Sends text <paramref name="message"/> to all websockets connections managed by this object.
        /// </summary>
        /// <param name="message">Text of message.</param>
        /// <param name="encoding">Encoding of message. <see cref="Encoding.UTF8"/> if null.</param>
        /// <param name="token">Cancellation token.</param>
        public async Task SendTextToAllAsync(string message, Encoding? encoding = null, CancellationToken token = default)
        {
            var sockets = _sockets;
            foreach (var socket in sockets)
            {
                await socket.SendTextAsync(message, encoding, token).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Websocket connections manager for <typeparamref name="THandler"/> websockets handler.
    /// </summary>
    /// <typeparam name="THandler">Type of websockets handler.</typeparam>
    public sealed class WsManager<THandler> : WsManager where THandler : WsHandler
    {
        /// <inheritdoc/>
        internal override void PreventExternalInheritance()
        {
            // Empty
        }
    }
}