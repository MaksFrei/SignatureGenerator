using SignatureGenerator.CLI.Commands.Abstractions;
using SignatureGenerator.CLI.Worker.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SignatureGenerator.CLI.Worker
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class CliWorker : ICliWorker
    {
        private readonly IReadOnlyDictionary<string, ICliCommand> commands;
        private readonly CancellationTokenSource cancellationTokenSource;
        private readonly IReadOnlyDictionary<string, Action> basicCommands;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="commands">Commands list</param>
        public CliWorker(IEnumerable<ICliCommand> commands)
        {
            this.commands = commands.ToDictionary(x => x.Name.ToLower());
            cancellationTokenSource = new CancellationTokenSource();
            basicCommands = new Dictionary<string, Action>
            {
                {"help", Help },
                {"stop", Stop }
            };
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Execute()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    Console.WriteLine("Write a command or enter help to see more");

                    var line = Console.ReadLine().ToLower();

                    if (commands.TryGetValue(line.ToLower(), out var cmd))
                        cmd.Invoke(cancellationTokenSource.Token);
                    else if (basicCommands.TryGetValue(line.ToLower(), out var basic))
                        basic.Invoke();
                    else
                        Console.WriteLine("Wrong command");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}{Environment.NewLine}Stacktrace: {ex.StackTrace}");
                }
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }
        
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Dispose()
        {
            cancellationTokenSource.Dispose();
        }

        private void Help()
        {
            var cmdsAsStrings = string.Join(Environment.NewLine, commands.Select(x
                 => x.Key +
                    (string.IsNullOrEmpty(x.Value.Description)
                        ? string.Empty
                        : Environment.NewLine + "    Description: " + x.Value.Description) + Environment.NewLine)
                .Concat(basicCommands.Select(x => x.Key + Environment.NewLine)));
            Console.WriteLine(@"Available commands: " + Environment.NewLine + cmdsAsStrings);
        }

    }
}
