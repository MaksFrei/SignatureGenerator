using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SignatureGenerator.CLI.Commands;
using SignatureGenerator.CLI.Commands.Abstractions;
using SignatureGenerator.CLI.Worker;
using SignatureGenerator.CLI.Worker.Abstractions;
using SignatureGenerator.Generator;

namespace SignatureGenerator.CLI
{
    public class Program
    {
        /// <summary>
        /// Sets up DI and runs CLI worker
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
           .AddLogging(builder => builder
               .AddConsole()
               .AddFilter(level => level >= LogLevel.Information))
           .AddSingleton<ICliWorker, CliWorker>()
           .AddTransient<ICliCommand, GenerateSignatureCommand>()
           .GeneratorModuleRegistration()
           .BuildServiceProvider();

            using var worker = serviceProvider.GetService<ICliWorker>();
            worker.Execute();
        }
    }
}
