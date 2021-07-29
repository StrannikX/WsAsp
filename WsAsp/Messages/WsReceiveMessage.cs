using System;
using System.Net.WebSockets;
using System.Text;

// ReSharper disable once CheckNamespace
namespace WsAsp
{
    /// <summary>
    /// Struct with information about websocket message.
    /// </summary>
    public readonly struct WsReceiveMessage
    {
        /// <summary>
        /// Creates message struct.
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        /// <param name="data">Message data.</param>
        /// <param name="dataLength">Message data length in bytes.</param>
        internal WsReceiveMessage(WebSocketMessageType messageType, byte[] data, int dataLength)
        {
            if (messageType == WebSocketMessageType.Close)
            {
                throw new ArgumentException("Can't create message object for close message!");
            }

            _type = messageType;
            DataLength = dataLength;
            _data = data;
        }

        /// <summary>
        /// Message type.
        /// </summary>
        private readonly WebSocketMessageType _type;

        /// <summary>
        /// Message data.
        /// </summary>
        private readonly byte[] _data;

        /// <summary>
        /// Length of message data in bytes.
        /// </summary>
        public int DataLength { get; }

        /// <summary>
        /// Is message have binary type.
        /// </summary>
        public bool IsBinary => _type == WebSocketMessageType.Binary;

        /// <summary>
        /// Is message have text type.
        /// </summary>
        public bool IsText => _type == WebSocketMessageType.Text;

        /// <summary>
        /// Raw message data.
        /// </summary>
        public ReadOnlyMemory<byte> Data => new ArraySegment<byte>(_data, 0, DataLength);

        /// <summary>
        /// Returns message data as string decoded by <paramref name="encoding"/>.
        /// </summary>
        /// <param name="encoding">Encoding of message. <see cref="Encoding.UTF8"/> if null.</param>
        /// <returns>Decoded message string.</returns>
        public string GetString(Encoding? encoding = null)
        {
            encoding ??= Encoding.UTF8;
            return encoding.GetString(_data, 0, DataLength);
        }
    }
}