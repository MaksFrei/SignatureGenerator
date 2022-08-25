using System;
using System.Collections.Generic;
using System.Threading;

namespace SignatureGenerator.Generator.Producers.CollectionProducers.Abstractions
{
    /// <summary>
    /// Consumes collection to produce it for other
    /// </summary>
    public interface ICollectionProducer<TConsumedData, TProducedData> : IDisposable
    {
        /// <summary>
        /// Flag for current work status
        /// </summary>
        bool DoesWorkDone { get; }

        /// <summary>
        /// Event that signals for external worker that work is done
        /// </summary>
        ManualResetEventSlim WorkIsDoneSyncEvent { get; }

        /// <summary>
        /// The way how data could be produced
        /// </summary>
        TProducedData ProducedData { get; }

        /// <summary>
        /// Event for stop/start work
        /// </summary>
        ManualResetEventSlim SyncEvent { get; }

        /// <summary>
        /// Sync events that may allow stopping filling of consumed collection for load balancing
        /// </summary>
        IReadOnlyCollection<ManualResetEventSlim> LoadBalancingEvents { get; }

        /// <summary>
        /// Sets event that signals for external worker that work is done
        /// </summary>
        /// <param name="syncEvent"></param>
        /// <returns></returns>
        public ICollectionProducer<TConsumedData, TProducedData>
            SetWorkIsDoneSyncEvent(ManualResetEventSlim syncEvent);

        /// <summary>
        /// Sets collection for be produced
        /// </summary>
        /// <param name="data">Produced collection</param>
        /// <returns></returns>
        public ICollectionProducer<TConsumedData, TProducedData>
            SetProducedData(TProducedData data);

        /// <summary>
        /// Sets events that may allow stopping filling of consumed collection for load balancing
        /// </summary>
        /// <param name="syncEvents"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ICollectionProducer<TConsumedData, TProducedData>
            SetLoadBalancingEvents(IReadOnlyCollection<ManualResetEventSlim> syncEvents);

        /// <summary>
        /// Starts work until whole collection end
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="consumedCollection">Collection to be consumed</param>
        /// <param name="doesConsumedCollectionFinishedCallback">Callback for checking work on completion of the consumed collection filling</param>
        /// <param name="cancellationToken"></param>
        void Produce(TConsumedData consumedData,
            Func<bool> doesConsumedCollectionFinishedCallback = null, 
            CancellationToken cancellationToken = default);
    }
}
