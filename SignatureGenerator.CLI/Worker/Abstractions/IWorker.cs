using System;

namespace SignatureGenerator.CLI.Worker.Abstractions
{
    /// <summary>
    /// Provides access to console commands
    /// </summary>
    internal interface ICliWorker : IDisposable
    {
        /// <summary>
        /// Runs in endless loop, until stop requested
        /// </summary>
        void Execute();

        /// <summary>
        /// Stops current command
        /// </summary>
        void Stop();
    }
}