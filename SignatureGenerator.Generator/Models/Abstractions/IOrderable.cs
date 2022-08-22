namespace SignatureGenerator.Generator.Models.Abstractions
{
    public interface ISorterable<T>
    {
        T Order { get; }
    }
}
