using SignatureGenerator.Generator.Models.Abstractions;

namespace SignatureGenerator.Generator.Models
{
    public record ByteChunk(int Order, byte[] Bytes) : ISorterable<int>, IByteRepresentationHolder;

    public record HashedChunk(int Order, string SHA256) : ISorterable<int>;
}
