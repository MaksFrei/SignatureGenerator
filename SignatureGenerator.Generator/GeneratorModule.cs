using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SignatureGenerator.Generator.Abstractions;
using SignatureGenerator.Generator.LoadBalancer;
using SignatureGenerator.Generator.LoadBalancer.Abstractions;
using SignatureGenerator.Generator.Models;
using SignatureGenerator.Generator.Producers.CollectionProducers;
using SignatureGenerator.Generator.Producers.CollectionProducers.Abstractions;
using SignatureGenerator.Generator.StreamProducers;
using SignatureGenerator.Generator.StreamProducers.Abstractions;
using System.Collections.Concurrent;

namespace SignatureGenerator.Generator
{
    /// <summary>
    /// Extension for DI
    /// </summary>
    public static class GeneratorModule
    {
        /// <summary>
        /// DI registration for Generator's dll
        /// </summary>
        /// <param name="services"></param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection GeneratorModuleRegistration(this IServiceCollection services)
        {            
            return services
                .AddTransient<IGenerator, Generator>()
                .AddSingleton<ICollectionProducerFactory, CollectionProducerFactory>()
                .AddSingleton<IStreamProducerFactory, StreamProducerFactory>()
                .AddTransient<ICollectionProducer<IProducerConsumerCollection<ByteChunk>, IProducerConsumerCollection<HashedChunk>>, BytesChunksToSHA256Producer>()
                .AddTransient<ICollectionProducer<IProducerConsumerCollection<HashedChunk>, ILogger<Generator>>, CollectionToLoggerProducer>()
                .AddTransient<IValuesLoadBalancer, ValueBasedLoadBalancer>()
                .AddTransient<StreamToConcurrentCollectionProducer, StreamToConcurrentCollectionProducer>();
        }
    }
}
