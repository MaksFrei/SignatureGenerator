using Microsoft.Extensions.Logging;
using SignatureGenerator.Generator.Models;
using SignatureGenerator.Generator.StreamProducers.Abstractions;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;

namespace SignatureGenerator.Generator.StreamProducers
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="TStreamType"></typeparam>
    class StreamToConcurrentCollectionProducer : IStreamToCollectionProducer
    {
        private readonly ILogger<StreamToConcurrentCollectionProducer> logger;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public bool DoesWorkDone { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IProducerConsumerCollection<ByteChunk> Collection { get; private set; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ManualResetEventSlim SyncEvent { get; }

        /// <summary>
        /// ctor
        /// </summary>
        public StreamToConcurrentCollectionProducer(ILogger<StreamToConcurrentCollectionProducer> logger)
        {
            SyncEvent = new ManualResetEventSlim(true);
            this.logger = logger;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="collection"><inheritdoc/></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IStreamToCollectionProducer SetProducedCollection(IProducerConsumerCollection<ByteChunk> collection)
        {
            if (collection is null) throw new ArgumentNullException(nameof(collection));

            Collection = collection;
            return this;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="sourceStream"><inheritdoc/></param>
        /// <param name="blockSize"><inheritdoc/></param>
        /// <param name="cancellationToken"></param>
        public void Produce(Stream sourceStream, int blockSize, CancellationToken cancellationToken = default)
        {
            if (Collection is null) throw new ArgumentNullException(nameof(Collection));
            if (blockSize <= 0) throw new ArgumentOutOfRangeException(nameof(blockSize));

            try
            {
                DoesWorkDone = false;
                using (sourceStream)
                {
                    int chunkNum = 0;
                    int bytesRead;
                    byte[] lastBuffer;

                    while (sourceStream.Position < sourceStream.Length)
                    {
                        if (cancellationToken.IsCancellationRequested) throw new OperationCanceledException();

                        SyncEvent.Wait(cancellationToken);

                        if (sourceStream.Length - sourceStream.Position <= blockSize)
                        {
                            bytesRead = (int)(sourceStream.Length - sourceStream.Position);
                        }
                        else
                        {
                            bytesRead = blockSize;
                        }

                        lastBuffer = new byte[bytesRead];
                        sourceStream.Read(lastBuffer, 0, bytesRead);


                        Collection.TryAdd(new ByteChunk(chunkNum++, lastBuffer));
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
