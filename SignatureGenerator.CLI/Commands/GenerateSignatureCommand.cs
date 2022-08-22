using SignatureGenerator.CLI.Commands.Abstractions;
using SignatureGenerator.Generator.Abstractions;
using SignatureGenerator.Generator.Utils;
using System;
using System.IO;
using System.Threading;

namespace SignatureGenerator.CLI.Commands
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class GenerateSignatureCommand : ICliCommand
    {
        private readonly IGenerator generator;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Name => "generate_signature";

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Description => "Generates file's signature based on SHA256 algorythm, and writes it down to the console";

        /// <summary>
        /// ctor
        /// </summary>
        public GenerateSignatureCommand(IGenerator generator)
        {
            this.generator = generator;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Invoke(CancellationToken cancelationToken = default)
        {
            string filePath;
            while (true)
            {
                Console.WriteLine("Enter file path to continue or stop to exit to main screen:");
                filePath = Console.ReadLine();

                if (filePath.ToLower() == "stop") return;
                else if (File.Exists(filePath)) break;

                Console.WriteLine("There is no file in {0}", filePath);
            }

            ulong blockSize;
            while (true)
            {
                Console.WriteLine("Enter block size in bytes to continue or stop to exit to main screen:");
                var command = Console.ReadLine();

                if (command.ToLower() == "stop")
                {
                    return;
                }
                else if (ulong.TryParse(command, out blockSize))
                {
                    if (RamInformator.TryToGetRamInfo(out var info) && info.AvailableRAM + blockSize * 10 < info.TotalRAM)
                    {
                        break;
                    }                        
                    else
                    {
                        Console.WriteLine("Entered block size may cause problems on processing, please enter a lesser value");
                        Console.WriteLine("RAM Total: {0}", info.TotalRAM);
                        Console.WriteLine("RAM Available: {0}", info.AvailableRAM);
                        Console.WriteLine("Entered block size: {0}", blockSize);
                        continue;
                    }
                }

                Console.WriteLine("Wrong block size", command);
            }

            Console.WriteLine("Command started");
            try
            {
                generator.GenerateSignature(filePath, blockSize, cancelationToken);
                Console.WriteLine("Command finished successfully");

            }
            catch(OutOfMemoryException ex)
            {
                Console.WriteLine("Out of memory, please select lesser block size and try again", ex.Message);
                throw;
            }
        }
    }
}
