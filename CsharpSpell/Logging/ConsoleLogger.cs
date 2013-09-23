using System;

namespace CsharpSpell.Logging
{
    /// <summary>
    /// Represents entity to print program's results at the console.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Prints message at the console.
        /// </summary>
        /// <param name="message">The message for printing.</param>
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Prints message at the console used string.Format.
        /// </summary>
        /// <param name="format">The format of printing string.</param>
        /// <param name="messages">The messages for printing.</param>
        public void LogFormat(string format, params object[] messages)
        {
            Console.WriteLine(format, messages);
        }
    }
}