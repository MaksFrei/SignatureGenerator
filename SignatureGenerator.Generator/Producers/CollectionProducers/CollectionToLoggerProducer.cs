using SignatureGenerator.Generator.Models;
using SignatureGenerator.Generator.Producers.CollectionProducers.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;
using SignatureGenerator.Generator.Utils;

namespace SignatureGenerator.Generator.Producers.CollectionProducers
{
    internal class CollectionToLoggerProducer : CollectionProducer<HashedChunk, ILogger<Generator>>
    {
        /// <summary>
        /// ctor
        /// </summary>
        public CollectionToLoggerProducer(ILogger<CollectionToLoggerProducer> logger)
            : base(logger)
        {
        }

        protected override void DoWork(HashedChunk item, CancellationToken cancellationToken)
        {
            ProducedData.LogStraightToConsole($"Block number: {item.Order}{Environment.NewLine}Hash: {item.SHA256}{Environment.NewLine}");
        }

        protected override void LoadBalancing(IProducerConsumerCollection<HashedChunk> consumedData, ILogger<Generator> producedData)
        {
            //todo: There may be some kind of RAM load balancer here, but we don't need it right now.
            return;
        }
    }
}
