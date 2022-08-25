using Microsoft.Extensions.Logging;
using System;
namespace SignatureGenerator.Generator.Utils
{
    internal static class LoggerExtentions
    {
        internal static void LogStraightToConsole(this ILogger logger, string value) => Console.WriteLine(value);
    }
    
}
