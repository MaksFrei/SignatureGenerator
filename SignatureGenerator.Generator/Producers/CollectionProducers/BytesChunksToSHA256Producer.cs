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
    public class BytesChunksToSHA256Producer
        : ICollectionProducer<IProducerConsumerCollection<ByteChunk>, IProducerConsumerCollection<HashedChunk>>
    {
        private readonly ILogger<BytesChunksToSHA256Producer> logger;

        /// <summary>
        /// Flag for current work status
        /// </summary>
        public bool DoesWorkDone { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IProducerConsumerCollection<HashedChunk> ProducedData { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ManualResetEventSlim SyncEvent { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public BytesChunksToSHA256Producer(ILogger<BytesChunksToSHA256Producer> logger)
        {
            SyncEvent = new ManualResetEventSlim(true);
            this.logger = logger;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data">Produced collection</param>
        /// <returns></returns>
        public ICollectionProducer<IProducerConsumerCollection<ByteChunk>, IProducerConsumerCollection<HashedChunk>>
            SetProducedData(IProducerConsumerCollection<HashedChunk> data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));

            ProducedData = data;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="consumedData"><inheritdoc/></param>
        /// <param name="doesConsumedCollectionFinishedCallback"><inheritdoc/></param>
        /// <param name="cancellationToken"><inheritdoc/></param>
        public void Produce(IProducerConsumerCollection<ByteChunk> consumedData, Func<bool> doesConsumedCollectionFinishedCallback, CancellationToken cancellationToken = default)
        {
            if (ProducedData is null) throw new ArgumentNullException(nameof(ProducedData));
            if (consumedData is null) throw new ArgumentNullException(nameof(consumedData));

            try
            {
                DoesWorkDone = false;
                using var sha = SHA256.Create();
                while (consumedData.Count > 0 || !doesConsumedCollectionFinishedCallback.Invoke())
                {
                    if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

                    SyncEvent.Wait(cancellationToken);
                    if (consumedData.TryTake(out var item))
                    {
                        while (!ProducedData.TryAdd(new HashedChunk(item.Order, Convert.ToHexString(item.Bytes)))
                             && !cancellationToken.IsCancellationRequested) ;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception: {ex.Message}{Environment.NewLine}Stacktrace: {ex.StackTrace}");
                return;
            }
            finally
            {
                DoesWorkDone = true;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            SyncEvent.Dispose();
        }
    }
}
