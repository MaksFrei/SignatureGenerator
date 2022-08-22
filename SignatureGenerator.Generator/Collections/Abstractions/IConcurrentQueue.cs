using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SignatureGenerator.Generator.Collections.Abstractions
{
    public interface IConcurrentQueue<T> : IProducerConsumerCollection<T>,
        IEnumerable<T>
    {
        void Enqueue(T item);
        bool TryDequeue(out T item);
    }
}