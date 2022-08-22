using SignatureGenerator.Generator.Models.Abstractions;

namespace SignatureGenerator.Generator.Models
{
    public record ByteChunk(int Order, byte[] Bytes) : ISorterable<int>, IByteRepresentationHolder;

    public class HashedChunk : ISorterable<int>
    {
        public int Order { get; set; }

        public HashedChunk(int order, string sha256)
        {
            Order = order;
            SHA256 = sha256;
        }

        public string SHA256 { get; set; }
    }
}
