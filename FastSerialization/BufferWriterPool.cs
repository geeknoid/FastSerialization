using System;
using System.Buffers;
using System.Collections.Concurrent;

namespace FastSerialization
{
    internal sealed class BufferWriterPool<T>
    {
        private readonly ConcurrentBag<ArrayBufferWriter<T>> _writers = new();

        public ArrayBufferWriter<T> Rent()
        {
            if (_writers.TryTake(out var result))
            {
                return result;
            }

            return new ArrayBufferWriter<T>();
        }

        public void Return(ArrayBufferWriter<T> writer)
        {
            writer.Clear();
            _writers.Add(writer);
        }
    }
}
