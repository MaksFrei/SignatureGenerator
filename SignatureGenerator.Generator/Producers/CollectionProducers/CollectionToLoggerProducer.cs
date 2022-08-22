using SignatureGenerator.Generator.Models;
using SignatureGenerator.Generator.Producers.CollectionProducers.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace SignatureGenerator.Generator.Producers.CollectionProducers
{
    internal class CollectionToLoggerProducer : ICollectionProducer<IProducerConsumerCollection<HashedChunk>, ILogger<Generator>>
    {
        private readonly ILogger<CollectionToLoggerProducer> logger;

        /// <summary>
        /// Flag for current work status
        /// </summary>
        public bool DoesWorkDone { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ILogger<Generator> ProducedData { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ManualResetEventSlim SyncEvent { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public CollectionToLoggerProducer(ILogger<CollectionToLoggerProducer> logger)
        {
            SyncEvent = new ManualResetEventSlim(true);
            this.logger = logger;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="collection">Produced collection</param>
        /// <returns></returns>
        public ICollectionProducer<IProducerConsumerCollection<HashedChunk>, ILogger<Generator>>
            SetProducedData(ILogger<Generator> data)
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
        public void Produce(IProducerConsumerCollection<HashedChunk> consumedData, Func<bool> doesConsumedCollectionFinishedCallback, CancellationToken cancellationToken = default)
        {
            if (ProducedData is null) throw new ArgumentNullException(nameof(ProducedData));
            if (consumedData is null) throw new ArgumentNullException(nameof(consumedData));

            try
            {
                DoesWorkDone = false;
                while (consumedData.Count > 0 || !doesConsumedCollectionFinishedCallback.Invoke())
                {
                    if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

                    SyncEvent.Wait(cancellationToken);
                    if (consumedData.TryTake(out var item))
                    {

                        //ProducedData.LogInformation($"Block number: {item.Order}{Environment.NewLine}Hash: {item.SHA256}{Environment.NewLine}");
                        Console.WriteLine($"Block number: {item.Order}{Environment.NewLine}Hash: {item.SHA256}{Environment.NewLine}");
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
