using System;
using Emotion.Standard.Logging;

namespace Solution12
{
    public class ConsoleLogger : LoggingProvider
    {
        public override void Log(MessageType type, string source, string message)
        {
            Console.WriteLine($"[{source}]: {type} - {message}");
        }

        public override void Dispose()
        {
            /* do noting */
        }
    }
}