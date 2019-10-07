using System;
using Adfectus.Logging;

namespace Solution12
{
    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated by [EngineBuilder]
    public class ErrorConsoleLogger : LoggingProvider
    {
        public override void Log(MessageType type, MessageSource source, string message)
        {
            if (type == MessageType.Trace || type == MessageType.Error)
            {
                Console.Write($"{type}: {source} - {message}");
            }
        }

        public override void Dispose()
        {
            /* do noting */
        }
    }
}