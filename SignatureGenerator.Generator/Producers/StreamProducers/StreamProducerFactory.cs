using Microsoft.Extensions.DependencyInjection;
using SignatureGenerator.Generator.StreamProducers.Abstractions;
using System;

namespace SignatureGenerator.Generator.StreamProducers
{
    internal class StreamProducerFactory : IStreamProducerFactory
    {
        private readonly IServiceProvider services;

        public StreamProducerFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public IStreamToCollectionProducer Create<T>()
            where T : IStreamToCollectionProducer
        {
            return services.GetService<T>();
        }
    }
}
