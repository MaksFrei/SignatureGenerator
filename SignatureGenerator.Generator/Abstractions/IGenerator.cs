using System.Threading;

namespace SignatureGenerator.Generator.Abstractions
{
    /// <summary>
    /// File's signature generator
    /// </summary>
    public interface IGenerator
    {
        /// <summary>
        /// Generates signature
        /// </summary>
        /// <param name="filePath">Path to a file</param>
        /// <param name="blockSize">Size for one block to generate signature</param>
        /// <param name="progressInfoCallback">Callback for each generated row</param>
        void GenerateSignature(string filePath, ulong blockSize, CancellationToken cancellationToken = default);
    }
}