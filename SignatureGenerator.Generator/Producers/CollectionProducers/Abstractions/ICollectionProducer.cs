using System;
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
        /// The way how data could be produced
        /// </summary>
        TProducedData ProducedData { get; }

        /// <summary>
        /// Event for stop/start work
        /// </summary>
        ManualResetEventSlim SyncEvent { get; }

        /// <summary>
        /// Sets collection for be produced
        /// </summary>
        /// <param name="data">Produced collection</param>
        /// <returns></returns>
        public ICollectionProducer<TConsumedData, TProducedData>
            SetProducedData(TProducedData data);

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
