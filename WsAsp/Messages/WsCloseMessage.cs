using System.Net.WebSockets;

// ReSharper disable once CheckNamespace
namespace WsAsp
{
    /// <summary>
    /// Structure with websocket closing information.
    /// </summary>
    public readonly struct WsCloseMessage
    {

        /// <summary>
        /// Creates web socket closing information struct.
        /// </summary>
        /// <param name="closeStatus">Close status.</param>
        /// <param name="closeStatusDescription">Close status description.</param>
        internal WsCloseMessage(WebSocketCloseStatus? closeStatus, string? closeStatusDescription)
        {
            CloseStatus = closeStatus;
            CloseStatusDescription = closeStatusDescription;
        }

        /// <summary>
        /// Web socket close status.
        /// </summary>
        public WebSocketCloseStatus? CloseStatus { get; }

        /// <summary>
        /// Web socket close status description.
        /// </summary>
        public string? CloseStatusDescription { get; }
    }
}