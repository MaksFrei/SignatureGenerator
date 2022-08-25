using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace SignatureGenerator.Generator.Producers.CollectionProducers.Abstractions
{
    internal abstract class CollectionProducer<TConsumedCollectionItemType, TProducedData>
        : ICollectionProducer<IProducerConsumerCollection<TConsumedCollectionItemType>, TProducedData>
    {
        private readonly ILogger<CollectionProducer<TConsumedCollectionItemType, TProducedData>> logger;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool DoesWorkDone { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ManualResetEventSlim WorkIsDoneSyncEvent { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TProducedData ProducedData { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ManualResetEventSlim SyncEvent { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IReadOnlyCollection<ManualResetEventSlim> LoadBalancingEvents { get; private set; }

        /// <summary>
        /// ctor
        /// </summary>
        public CollectionProducer(ILogger<CollectionProducer<TConsumedCollectionItemType, TProducedData>> logger)
        {
            SyncEvent = new ManualResetEventSlim(true);
            this.logger = logger;
        }

        /// <summary>
        /// Implementation of the main logic
        /// </summary>
        abstract protected void DoWork(TConsumedCollectionItemType item, CancellationToken cancellationToken);


        /// <summary>
        /// Performace control logic
        /// </summary>
        protected abstract void LoadBalancing(IProducerConsumerCollection<TConsumedCollectionItemType> consumedData, TProducedData producedData);

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="data">Produced collection</param>
        /// <returns></returns>
        public ICollectionProducer<IProducerConsumerCollection<TConsumedCollectionItemType>, TProducedData>
            SetProducedData(TProducedData data)
        {
            if (data is null) throw new ArgumentNullException(nameof(data));

            ProducedData = data;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="syncEvents"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ICollectionProducer<IProducerConsumerCollection<TConsumedCollectionItemType>, TProducedData>
            SetLoadBalancingEvents(IReadOnlyCollection<ManualResetEventSlim> syncEvents)
        {
            if (syncEvents is null) throw new ArgumentNullException(nameof(syncEvents));

            LoadBalancingEvents = syncEvents;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="syncEvent"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ICollectionProducer<IProducerConsumerCollection<TConsumedCollectionItemType>, TProducedData>
            SetWorkIsDoneSyncEvent(ManualResetEventSlim syncEvent)
        {
            if(syncEvent is null) throw new ArgumentNullException(nameof(syncEvent));
            if (syncEvent.Equals(SyncEvent)) throw new ArgumentOutOfRangeException(nameof(syncEvent));

            WorkIsDoneSyncEvent = syncEvent;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="consumedData"><inheritdoc/></param>
        /// <param name="doesConsumedCollectionFinishedCallback"><inheritdoc/></param>
        /// <param name="cancellationToken"><inheritdoc/></param>
        public void Produce(IProducerConsumerCollection<TConsumedCollectionItemType> consumedData, Func<bool> doesConsumedCollectionFinishedCallback, CancellationToken cancellationToken = default)
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
                    LoadBalancing(consumedData, ProducedData);
                    if (consumedData.TryTake(out var item))
                    {
                        DoWork(item, cancellationToken);
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
                WorkIsDoneSyncEvent?.Set();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public virtual void Dispose()
        {
            SyncEvent.Dispose();
        }

    }
}
