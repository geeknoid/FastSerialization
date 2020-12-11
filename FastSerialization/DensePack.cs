using System;
using System.Buffers;
using MessagePack;

namespace FastSerialization
{
    public static class DensePack
    {
        private static readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard
            .WithOmitAssemblyVersion(true)
            .WithCompression(MessagePackCompression.Lz4BlockArray);

        private static readonly BufferWriterPool<byte> _pool = new();

        /// <summary>
        /// Serialize an object to a compressed MessagePack format.
        /// </summary>
        /// <returns>
        /// A buffer writer containing the serialized data. Return this to the
        /// pool using the Return function when you're done with it.
        /// </returns>
        public static ArrayBufferWriter<byte> Serialize<T>(T data)
        {
            var br = _pool.Rent();
            MessagePackSerializer.Serialize(br, data, _options);
            return br;
        }

        /// <summary>
        /// Returns a buffer writer to the pool.
        /// </summary>
        public static void Return(ArrayBufferWriter<byte> writer)
        {
            _pool.Return(writer);
        }

        /// <summary>
        /// Deserialize a compressed MessagePack block of data into an object
        /// </summary>
        public static T Deserialze<T>(ReadOnlyMemory<byte> data)
        {
            return MessagePackSerializer.Deserialize<T>(data, _options);
        }
    }
}
