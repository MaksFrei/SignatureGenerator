using System.Threading;

namespace SignatureGenerator.CLI.Commands.Abstractions
{
    /// <summary>
    /// CLI command
    /// </summary>
    internal interface ICliCommand
    {
        /// <summary>
        /// Command name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Command description
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Run the command
        /// </summary>
        /// <param name="cancelationToken"></param>
        void Invoke(CancellationToken cancelationToken = default);
    }
}
