using Microsoft.Extensions.Logging;
using SignatureGenerator.Generator.Models;
using SignatureGenerator.Generator.Producers.CollectionProducers.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Threading;

namespace SignatureGenerator.Generator.Producers.CollectionProducers
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class BytesChunksToSHA256Producer
        : CollectionProducer<ByteChunk, IProducerConsumerCollection<HashedChunk>>
    {
        private readonly SHA256 hasher = SHA256.Create();

        /// <summary>
        /// ctor
        /// </summary>
        public BytesChunksToSHA256Producer(ILogger<BytesChunksToSHA256Producer> logger)
            : base(logger)
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Dispose()
        {
            hasher.Dispose();
            base.Dispose();
        }

        protected override void DoWork(ByteChunk item, CancellationToken cancellationToken)
        {
            while (!ProducedData.TryAdd(new HashedChunk(item.Order, Convert.ToHexString(hasher.ComputeHash(item.Bytes))))
                 && !cancellationToken.IsCancellationRequested) ;
        }

        protected override void LoadBalancing(IProducerConsumerCollection<ByteChunk> consumedData, IProducerConsumerCollection<HashedChunk> producedData)
        {
            if (!isEnough())
            {
                foreach (var @event in LoadBalancingEvents)
                    @event.Reset();
            }
            else
            {
                foreach (var @event in LoadBalancingEvents)
                    @event.Set();
            }

            bool isEnough()
            {
                return producedData.Count == 0
                    || producedData.Count <= consumedData.Count
                    && 100 / ((double)consumedData.Count / (consumedData.Count - producedData.Count)) <= 10;
            }
        }
    }
}
