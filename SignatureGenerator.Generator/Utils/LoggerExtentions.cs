using Microsoft.Extensions.Logging;
using System;
namespace SignatureGenerator.Generator.Utils
{
    public static class LoggerExtentions
    {
        public static void LogStraightToConsole(this ILogger logger, string value) => Console.WriteLine(value);
    }
    
}
