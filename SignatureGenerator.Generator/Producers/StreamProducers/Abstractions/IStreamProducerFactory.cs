namespace SignatureGenerator.Generator.StreamProducers.Abstractions
{
    /// <summary>
    /// Stream readers factory
    /// </summary>
    public interface IStreamProducerFactory
    {
        /// <summary>
        /// Create instance of asked type
        /// </summary>
        /// <typeparam name="T">Producer's type</typeparam>
        /// <param name="args">Array of produser's constructor params, sorted as they were entered into the code</param>
        /// <returns></returns>
        IStreamToCollectionProducer Create<T>()
            where T : IStreamToCollectionProducer;
    }
}