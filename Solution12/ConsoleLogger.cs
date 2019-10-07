﻿using System;
using Adfectus.Logging;

namespace Solution12
{
    // ReSharper disable once ClassNeverInstantiated.Global - Instantiated by [EngineBuilder]
    public class ConsoleLogger : LoggingProvider
    {
        public override void Log(MessageType type, MessageSource source, string message)
        {
            Console.WriteLine($"[{source}]: {type} - {message}");
        }

        public override void Dispose()
        {
            /* do noting */
        }
    }
}