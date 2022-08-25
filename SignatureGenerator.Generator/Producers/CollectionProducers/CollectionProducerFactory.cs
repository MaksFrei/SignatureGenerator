using Microsoft.Extensions.DependencyInjection;
using SignatureGenerator.Generator.Producers.CollectionProducers.Abstractions;
using System;

namespace SignatureGenerator.Generator.Producers.CollectionProducers
{
    internal class CollectionProducerFactory : ICollectionProducerFactory
    {
        private readonly IServiceProvider services;

        public CollectionProducerFactory(IServiceProvider services)
        {
            this.services = services;
        }

        public ICollectionProducer<TConsumedDataType, TProducedDataType>
            Create<TProducerType, TConsumedDataType, TProducedDataType>()
            where TProducerType : ICollectionProducer<TConsumedDataType, TProducedDataType>
        {
            return services.GetService<ICollectionProducer<TConsumedDataType, TProducedDataType>>();
        }
    }
}
