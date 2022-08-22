namespace SignatureGenerator.Generator.Producers.CollectionProducers.Abstractions
{
    public interface ICollectionProducerFactory
    {
        /// <summary>
        /// Create instance of asked type
        /// </summary>
        /// <typeparam name="TProducerType">Producer's type</typeparam>
        /// <typeparam name="TConsumedCollectionType"></typeparam>
        /// <typeparam name="TProducedCollection"></typeparam>
        /// <param name="args">Array of produser's constructor params, sorted as they were entered into the code</param>
        /// <returns></returns>
        ICollectionProducer<TConsumedCollectionType, TProducedCollection>
            Create<TProducerType, TConsumedCollectionType, TProducedCollection>()
            where TProducerType : ICollectionProducer<TConsumedCollectionType, TProducedCollection>;
    }
}
