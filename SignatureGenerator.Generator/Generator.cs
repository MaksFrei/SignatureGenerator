using Microsoft.Extensions.Logging;
using SignatureGenerator.Generator.Abstractions;
using SignatureGenerator.Generator.Models;
using SignatureGenerator.Generator.Producers.CollectionProducers;
using SignatureGenerator.Generator.Producers.CollectionProducers.Abstractions;
using SignatureGenerator.Generator.Queue;
using SignatureGenerator.Generator.StreamProducers;
using SignatureGenerator.Generator.StreamProducers.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace SignatureGenerator.Generator
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class Generator : IGenerator
    {
        private const string TempFilesPath = "temp/";
        private readonly IStreamProducerFactory streamProducerFactory;
        private readonly ICollectionProducerFactory collectionProducerFactory;
        private readonly ILogger<Generator> logger;
        private readonly int coresCount = Environment.ProcessorCount;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="streamProducerFactory"></param>
        public Generator(IStreamProducerFactory streamProducerFactory,
            ICollectionProducerFactory collectionProducerFactory,
            ILoggerFactory loggerFactory)
        {
            this.streamProducerFactory = streamProducerFactory;
            this.collectionProducerFactory = collectionProducerFactory;
            this.logger = loggerFactory.CreateLogger<Generator>();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="filePath"><inheritdoc/></param>
        /// <param name="blockSize"><inheritdoc/></param>
        /// <param name="progressInfoCallback"><inheritdoc/></param>
        public void GenerateSignature(string filePath, ulong blockSize, CancellationToken cancellationToken = default)
        {
            using var stream = new FileStream(filePath, FileMode.Open);
            var eventsForDiscBalance = new List<ManualResetEventSlim>();
            var waitUntillWorkIsDone = new ManualResetEventSlim(false);

            var rawBlocksQueue = new ConcurrentQueue<ByteChunk>();
            using var fileToRawBlocksProducer = streamProducerFactory.Create<StreamToConcurrentCollectionProducer>()
                .SetProducedCollection(rawBlocksQueue);
            var fileToRawBlocksFinishedCallback = () => fileToRawBlocksProducer.DoesWorkDone;
            eventsForDiscBalance.Add(fileToRawBlocksProducer.SyncEvent);

            var hashedBlocks = new SortedConcurrentQueue<HashedChunk, int>(0, (x) => ++x);
            var rawBlocksToHashProducers = Enumerable.Range(0, coresCount).Select(_ =>
                     collectionProducerFactory
                    .Create<BytesChunksToSHA256Producer, IProducerConsumerCollection<ByteChunk>, IProducerConsumerCollection<HashedChunk>>()
                    .SetProducedData(hashedBlocks)
                    .SetLoadBalancingEvents(eventsForDiscBalance))
                .Select(p => p.SetProducedData(hashedBlocks))
                .ToList();
            var rawBlocksToHashFinishedCallback = () => rawBlocksToHashProducers.All(p => p.DoesWorkDone);

            using var hashedBlocksToLogProducer = collectionProducerFactory
                .Create<CollectionToLoggerProducer, IProducerConsumerCollection<HashedChunk>, ILogger<Generator>>()
                .SetProducedData(logger)
                .SetWorkIsDoneSyncEvent(waitUntillWorkIsDone);

            var filetoRawBlocksTask = new Thread(() => fileToRawBlocksProducer.Produce(stream, (int)blockSize, cancellationToken));
            var rawBlocksToHashTasks = rawBlocksToHashProducers.Select(p => new Thread(() => p.Produce(rawBlocksQueue, fileToRawBlocksFinishedCallback, cancellationToken)));
            var hashToLogTask = new Thread(() => hashedBlocksToLogProducer.Produce(hashedBlocks, rawBlocksToHashFinishedCallback, cancellationToken));

            filetoRawBlocksTask.Start();
            foreach (var task in rawBlocksToHashTasks)
                task.Start();
            hashToLogTask.Start();

            waitUntillWorkIsDone.Wait();

            //Not sure that this is nessesary
            foreach (var producer in rawBlocksToHashProducers)
                producer.Dispose();
        }
    }
}
