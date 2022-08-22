using SignatureGenerator.Generator.Models;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace SignatureGenerator.Generator.StreamProducers.Abstractions
{
    /// <summary>
    /// Consumes IO stream to some concurrent collection of <see cref="ByteChunk"/>
    /// </summary>
    public interface IStreamToCollectionProducer : IDisposable
    {
        /// <summary>
        /// Flag for current work status
        /// </summary>
        bool DoesWorkDone { get; }

        /// <summary>
        /// Concurrent safe collection
        /// </summary>
        IProducerConsumerCollection<ByteChunk> Collection { get; }

        /// <summary>
        /// Event for stop/start work
        /// </summary>
        ManualResetEventSlim SyncEvent { get; }

        /// <summary>
        /// Sets collection for be produced
        /// </summary>
        /// <param name="collection">Produced collection</param>
        /// <returns></returns>
        public IStreamToCollectionProducer SetProducedCollection(IProducerConsumerCollection<ByteChunk> collection);

        /// <summary>
        /// Starts work until stream's end
        /// </summary>
        /// <param name="sourceStream">Source stream, will be disposed after work willl be done</param>
        /// <param name="blockSize">bytes to be read</param>
        void Produce(Stream sourceStream, int blockSize, CancellationToken cancellationToken = default);
    }
}
