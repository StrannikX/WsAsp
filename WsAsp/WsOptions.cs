using System;

namespace WsAsp
{
    /// <summary>
    /// Websockets options.
    /// </summary>
    public class WsOptions
    {
        /// <summary>
        /// Default size in bytes of websocket reading buffer. 64 Kb by default;
        /// </summary>
        public const int DefaultBufferSize = 64 * 1024;

        /// <summary>
        /// Default max size in bytes of websocket reading buffer. 1 Mb by default;
        /// </summary>
        public const int DefaultMaxBufferSize = 1024 * 1024;

        /// <summary>
        /// Size in bytes of websocket reading buffer.
        /// </summary>
        private int _bufferSize = DefaultBufferSize;

        /// <summary>
        /// Max size in bytes of websocket reading buffer.
        /// </summary>
        private int _maxBufferSize = DefaultMaxBufferSize;

        /// <summary>
        /// Size in bytes of websocket reading buffer.
        /// </summary>
        public int BufferSize
        {
            get => _bufferSize;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException($"{nameof(BufferSize)} value should be positive!");
                }

                if (value > MaxBufferSize)
                {
                    throw new ArgumentException($"{nameof(BufferSize)} should be not greater then {MaxBufferSize}");
                }

                _bufferSize = value;
            }
        }

        /// <summary>
        /// Max size in bytes of websocket reading buffer.
        /// </summary>
        public int MaxBufferSize
        {
            get => _maxBufferSize;
            set
            {
                if (value <= BufferSize)
                {
                    throw new ArgumentException($"{MaxBufferSize} should be greater then {nameof(BufferSize)}");
                }

                _maxBufferSize = value;
            }
        }
    }
}